# Deployment

Minigun deploys from GitHub Actions when a change is merged into `main`.

GitHub Actions builds one ARM64 release archive. It connects to the server with
Tailscale SSH and uploads the archive. GitHub does not store a server SSH key.

The server extracts each release into a new directory. It then points the
application at the new release and restarts the service. The server checks the
application over HTTPS. If the check fails, it starts the previous release.
The server keeps the five most recent releases.

The deployment user can upload a release and run the deployment script. It
cannot run other commands as root.

Application settings are stored on the server. They are not included in a
release archive.

Certificates are also managed on the server. Certbot renews the certificate and
restarts Minigun after a successful renewal.

## Files

- `bootstrap-server.sh` sets up a new server.
- `deploy-release.sh` installs a release and checks that it works.
- `minigun.service` runs Minigun with systemd.
- `setup-certificates.sh` installs the certificate and its renewal job.
- `certbot-renew.cron` runs the certificate renewal check.
- `restart-minigun-after-certificate-renewal.sh` restarts Minigun after renewal.
