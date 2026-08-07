#!/usr/bin/env bash

set -Eeuo pipefail

readonly APP_NAME="minigun"
readonly RELEASE_ROOT="/opt/minigun/releases"
readonly CURRENT_LINK="/opt/minigun/current"
readonly UPLOAD_ROOT="/var/lib/minigun-deploy/uploads"
readonly LOCK_FILE="/run/lock/minigun-deploy.lock"
readonly HEALTH_HOST="minigun.proredcat.xyz"
readonly HEALTH_URL="https://${HEALTH_HOST}/"
readonly RELEASES_TO_KEEP=5

fail() {
  echo "minigun deploy: $*" >&2
  exit 1
}

if [[ ${EUID} -ne 0 ]]; then
  fail "must run as root"
fi

if [[ $# -ne 1 || ! $1 =~ ^[0-9a-f]{40}$ ]]; then
  fail "usage: $0 <40-character git SHA>"
fi

readonly RELEASE_ID="$1"
readonly ARCHIVE="${UPLOAD_ROOT}/minigun-${RELEASE_ID}.tar.gz"
readonly RELEASE_DIR="${RELEASE_ROOT}/${RELEASE_ID}"
readonly STAGING_DIR="${RELEASE_ROOT}/.${RELEASE_ID}.staging"

exec 9>"${LOCK_FILE}"
flock -n 9 || fail "another deployment is already running"

[[ -f ${ARCHIVE} ]] || fail "release archive does not exist: ${ARCHIVE}"
[[ ! -e ${RELEASE_DIR} ]] || fail "release already exists: ${RELEASE_ID}"

archive_owner=$(stat -c '%U' "${ARCHIVE}")
[[ ${archive_owner} == "minigun-deploy" ]] || fail "release archive must be owned by minigun-deploy"

if tar -tzf "${ARCHIVE}" | awk '
  /^\// { bad=1 }
  /(^|\/)\.\.($|\/)/ { bad=1 }
  END { exit bad ? 0 : 1 }
'; then
  fail "release archive contains an unsafe path"
fi

rm -rf -- "${STAGING_DIR}"
install -d -o root -g root -m 0755 "${STAGING_DIR}"
tar -xzf "${ARCHIVE}" --no-same-owner --no-same-permissions -C "${STAGING_DIR}"

[[ -f ${STAGING_DIR}/Minigun ]] || fail "release does not contain the Minigun executable"
chmod 0755 "${STAGING_DIR}/Minigun"
chown -R root:root "${STAGING_DIR}"
mv "${STAGING_DIR}" "${RELEASE_DIR}"
rm -f -- "${ARCHIVE}"

previous_release=$(readlink -f "${CURRENT_LINK}" 2>/dev/null || true)
next_link="${CURRENT_LINK}.${RELEASE_ID}"
ln -s "${RELEASE_DIR}" "${next_link}"
mv -Tf "${next_link}" "${CURRENT_LINK}"

rollback() {
  echo "minigun deploy: health check failed; rolling back to ${previous_release:-<none>}" >&2
  if [[ -n ${previous_release} && -d ${previous_release} ]]; then
    rollback_link="${CURRENT_LINK}.rollback"
    ln -s "${previous_release}" "${rollback_link}"
    mv -Tf "${rollback_link}" "${CURRENT_LINK}"
    systemctl restart "${APP_NAME}"
  else
    systemctl stop "${APP_NAME}" || true
  fi
  exit 1
}

if ! systemctl restart "${APP_NAME}"; then
  rollback
fi

healthy=false
for _ in {1..30}; do
  if curl --fail --silent --output /dev/null \
    --resolve "${HEALTH_HOST}:443:127.0.0.1" \
    "${HEALTH_URL}"; then
    healthy=true
    break
  fi
  sleep 1
done

[[ ${healthy} == true ]] || rollback

mapfile -t old_releases < <(
  find "${RELEASE_ROOT}" -mindepth 1 -maxdepth 1 -type d ! -name '.*' -printf '%T@ %p\n' \
    | sort -nr \
    | awk -v keep="${RELEASES_TO_KEEP}" 'NR > keep { sub(/^[^ ]+ /, ""); print }'
)

for old_release in "${old_releases[@]}"; do
  [[ ${old_release} == "${RELEASE_ROOT}/"* ]] || continue
  [[ ${old_release} != "$(readlink -f "${CURRENT_LINK}")" ]] || continue
  rm -rf -- "${old_release}"
done

echo "minigun deploy: release ${RELEASE_ID} is healthy and active"
systemctl --no-pager --full status "${APP_NAME}"
