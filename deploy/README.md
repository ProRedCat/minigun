# Minigun deployments

Minigun is deployed as an immutable, versioned release over Tailscale SSH.
GitHub Actions builds the application, uploads one archive, and asks the server
to activate it. The server retains the five newest releases.

## Server layout

- `/opt/minigun/releases/<git-sha>` contains immutable releases.
- `/opt/minigun/current` points to the active release.
- `/etc/minigun/minigun.env` contains runtime secrets and is readable only by
  root.
- `/var/lib/minigun-deploy/uploads` is the deployment account's private upload
  directory.
- `/usr/local/sbin/minigun-deploy-release` validates, activates, health-checks,
  and rolls back releases.

## Tailnet policy

Define both tags before assigning `tag:minigun-prod` to the server or starting
the GitHub Actions integration. Merge these sections into the existing policy;
do not duplicate top-level keys.

```json
{
  "tagOwners": {
    "tag:minigun-prod": ["autogroup:admin"],
    "tag:minigun-ci": ["autogroup:admin"]
  },
  "grants": [
    {
      "src": ["tag:minigun-ci"],
      "dst": ["tag:minigun-prod"],
      "ip": ["tcp:22"]
    }
  ],
  "ssh": [
    {
      "action": "accept",
      "src": ["tag:minigun-ci"],
      "dst": ["tag:minigun-prod"],
      "users": ["minigun-deploy"]
    },
    {
      "action": "check",
      "src": ["autogroup:member"],
      "dst": ["tag:minigun-prod"],
      "users": ["ec2-user"]
    }
  ]
}
```

An existing allow-all grant can give `tag:minigun-ci` more access than the
grant above suggests. Review or replace broad grants before enabling the CI
identity.

After saving the policy, tag the server:

```bash
sudo tailscale up \
  --ssh \
  --hostname=minigun-prod \
  --advertise-tags=tag:minigun-prod
```

## GitHub workload identity

Create an OpenID Connect credential on the Tailscale **Trust credentials**
page with:

- Issuer: GitHub Actions
- Subject: `repo:ProRedCat/minigun:environment:production`
- Scope: `auth_keys` with write access
- Tag: `tag:minigun-ci`

Store the generated, non-secret values as `production` environment variables:

```bash
gh variable set TS_OAUTH_CLIENT_ID --env production --repo ProRedCat/minigun
gh variable set TS_AUDIENCE --env production --repo ProRedCat/minigun
```

The workflow requests a GitHub OIDC token and creates an ephemeral Tailscale
node for each deployment. No reusable Tailscale or SSH credential is stored in
GitHub.

## HTTPS certificates

TLS is configured separately from application releases. The setup script
installs Certbot when needed, obtains the initial certificate using the
standalone HTTP-01 challenge, and installs the twice-daily renewal schedule.

DNS must already point `minigun.proredcat.xyz` to the server and public TCP port
80 must be open before obtaining the initial certificate:

```bash
sudo ./deploy/setup-certificates.sh you@example.com
```

The email argument is required only when obtaining the first certificate. On an
already configured server, run the script without an argument to install or
refresh the scheduler and deployment hook.

The renewal job runs at midnight and midday UTC. After a certificate is
successfully renewed, Certbot restarts Minigun so Kestrel loads the new
certificate. Routine checks that do not renew a certificate do not restart the
application.

Test the HTTP-01 renewal path after provisioning:

```bash
sudo certbot renew --dry-run
```

## Server bootstrap

The idempotent bootstrap creates the deployment account, directories, systemd
unit, restricted sudo rule, and release command. On a brand-new server the
service remains stopped until the first artifact is deployed.

```bash
sudo ./deploy/bootstrap-server.sh
```

Create the root-only runtime configuration before the first deployment:

```bash
sudo install -d -o root -g root -m 0700 /etc/minigun
read -rsp "Raygun API key: " raygun_key && echo
printf 'Serilog__WriteTo__1__Args__applicationKey=%s\n' "$raygun_key" \
  | sudo tee /etc/minigun/minigun.env >/dev/null
unset raygun_key
sudo chown root:root /etc/minigun/minigun.env
sudo chmod 0600 /etc/minigun/minigun.env
```

## Recovery

SSM remains the break-glass access path. To activate a retained release:

```bash
release=/opt/minigun/releases/<git-sha>
sudo ln -s "$release" /opt/minigun/current.manual
sudo mv -Tf /opt/minigun/current.manual /opt/minigun/current
sudo systemctl restart minigun
curl --fail --resolve minigun.proredcat.xyz:443:127.0.0.1 \
  https://minigun.proredcat.xyz/ --output /dev/null
```

Do not remove public SSH access or the old GitHub EC2 secrets until a deployment
from a tagged GitHub Actions runner has completed successfully and SSM recovery
has been retested.
