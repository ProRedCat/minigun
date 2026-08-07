#!/usr/bin/env bash

set -Eeuo pipefail

readonly SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
readonly RELEASE_ROOT="/opt/minigun/releases"
readonly CURRENT_LINK="/opt/minigun/current"
readonly ENV_FILE="/etc/minigun/minigun.env"
readonly SERVICE_FILE="/etc/systemd/system/minigun.service"
readonly DEPLOY_COMMAND="/usr/local/sbin/minigun-deploy-release"

fail() {
  echo "minigun bootstrap: $*" >&2
  exit 1
}

[[ ${EUID} -eq 0 ]] || fail "must run as root"
[[ -f ${SCRIPT_DIR}/minigun.service ]] || fail "minigun.service is missing"
[[ -f ${SCRIPT_DIR}/deploy-release.sh ]] || fail "deploy-release.sh is missing"

if ! id minigun-deploy >/dev/null 2>&1; then
  useradd --create-home --shell /bin/bash --comment "Minigun deployment account" minigun-deploy
fi

install -d -o root -g root -m 0755 /opt/minigun "${RELEASE_ROOT}"
install -d -o root -g root -m 0700 /etc/minigun
install -d -o minigun-deploy -g minigun-deploy -m 0700 /var/lib/minigun-deploy/uploads
install -o root -g root -m 0755 "${SCRIPT_DIR}/deploy-release.sh" "${DEPLOY_COMMAND}"
install -o root -g root -m 0644 "${SCRIPT_DIR}/minigun.service" "${SERVICE_FILE}"

sudoers_file=$(mktemp)
trap 'rm -f -- "${sudoers_file}"' EXIT
printf 'minigun-deploy ALL=(root) NOPASSWD: %s *\n' "${DEPLOY_COMMAND}" > "${sudoers_file}"
chmod 0440 "${sudoers_file}"
visudo -cf "${sudoers_file}" >/dev/null
install -o root -g root -m 0440 "${sudoers_file}" /etc/sudoers.d/minigun-deploy

systemctl daemon-reload
systemctl enable minigun

if [[ -x ${CURRENT_LINK}/Minigun ]]; then
  [[ -f ${ENV_FILE} ]] || fail "runtime configuration is missing: ${ENV_FILE}"
  systemctl restart minigun

  healthy=false
  for _ in {1..30}; do
    if curl --fail --silent --output /dev/null \
      --resolve minigun.proredcat.xyz:443:127.0.0.1 \
      https://minigun.proredcat.xyz/; then
      healthy=true
      break
    fi
    sleep 1
  done

  [[ ${healthy} == true ]] || fail "existing release failed its health check"
  echo "minigun bootstrap: existing release is healthy"
else
  echo "minigun bootstrap: ready for the first release"
  echo "minigun bootstrap: create ${ENV_FILE} before deploying"
  echo "minigun bootstrap: run setup-certificates.sh before deploying"
fi
