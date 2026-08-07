#!/usr/bin/env bash

set -Eeuo pipefail

readonly SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
readonly DOMAIN="minigun.proredcat.xyz"
readonly CERTIFICATE="/etc/letsencrypt/live/${DOMAIN}/fullchain.pem"
readonly RENEWAL_HOOK="/etc/letsencrypt/renewal-hooks/deploy/restart-minigun"
readonly CRON_FILE="/etc/cron.d/certbot-renew"

fail() {
  echo "minigun certificates: $*" >&2
  exit 1
}

[[ ${EUID} -eq 0 ]] || fail "must run as root"
[[ -f ${SCRIPT_DIR}/restart-minigun-after-certificate-renewal.sh ]] || fail "renewal hook is missing"
[[ -f ${SCRIPT_DIR}/certbot-renew.cron ]] || fail "renewal schedule is missing"

if ! command -v certbot >/dev/null 2>&1; then
  dnf install -y certbot
fi

install -d -o root -g root -m 0755 /etc/letsencrypt/renewal-hooks/deploy
install -o root -g root -m 0755 \
  "${SCRIPT_DIR}/restart-minigun-after-certificate-renewal.sh" \
  "${RENEWAL_HOOK}"
install -o root -g root -m 0644 "${SCRIPT_DIR}/certbot-renew.cron" "${CRON_FILE}"
systemctl enable --now crond

if [[ ! -f ${CERTIFICATE} ]]; then
  [[ $# -eq 1 && -n $1 ]] || fail "usage for a new server: $0 <certificate-contact-email>"
  certbot certonly \
    --standalone \
    --non-interactive \
    --agree-tos \
    --email "$1" \
    --domain "${DOMAIN}"
fi

certbot certificates --cert-name "${DOMAIN}"
echo "minigun certificates: certificate and automatic renewal are configured"
