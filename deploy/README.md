# Minigun deployments

Minigun is deployed from GitHub Actions as an immutable, versioned release over
Tailscale SSH. No reusable SSH or Tailscale credential is stored in GitHub.

## How it works

1. GitHub Actions builds and packages the application.
2. An ephemeral CI node joins the tailnet as `tag:minigun-ci`.
3. The artifact is uploaded to the restricted `minigun-deploy` account on the
   server tagged `tag:minigun-prod`.
4. The server validates and activates the release, then checks application
   health. A failed release is rolled back automatically.
5. The five newest releases are retained.

Deployments run only from `main` through the protected `production` GitHub
environment.

## External configuration

Tailscale policy and GitHub's OIDC trust configuration are managed outside this
repository. They must provide the following contract:

- `tag:minigun-ci` may reach TCP port 22 on `tag:minigun-prod`.
- Tailscale SSH permits that CI identity to connect as `minigun-deploy` only.
- The OIDC trust credential accepts this repository's `production` environment,
  can create ephemeral auth keys, and may apply `tag:minigun-ci`.
- The `production` environment defines `TS_OAUTH_CLIENT_ID` and `TS_AUDIENCE`.

Keep the broader tailnet access policy, human administration rules, and
break-glass access procedure in the relevant private administration systems.

## Server setup

Run these scripts once when provisioning a server, from a checkout of this
repository:

```bash
sudo ./deploy/setup-certificates.sh you@example.com
sudo ./deploy/bootstrap-server.sh
```

`setup-certificates.sh` obtains the initial Let's Encrypt certificate and
installs twice-daily renewal. DNS must already point `minigun.proredcat.xyz` to
the server, and public TCP port 80 must remain reachable for HTTP-01 renewal.
After a successful renewal, Certbot restarts the application to load the new
certificate. Verify renewal after provisioning with:

```bash
sudo certbot renew --dry-run
```

`bootstrap-server.sh` idempotently creates the deployment account, directories,
systemd unit, restricted sudo rule, and server-side release command. On a new
server, the service remains stopped until its first deployment.

Before that deployment, create the root-owned runtime environment file at
`/etc/minigun/minigun.env` with mode `0600`. It must contain:

```dotenv
Serilog__WriteTo__1__Args__applicationKey=<Raygun API key>
```

## Server layout

- `/opt/minigun/releases/<git-sha>` contains immutable releases.
- `/opt/minigun/current` points to the active release.
- `/etc/minigun/minigun.env` contains runtime secrets.
- `/var/lib/minigun-deploy/uploads` receives deployment archives.
- `/usr/local/sbin/minigun-deploy-release` performs validated activation,
  health checking, rollback, and release cleanup.

The deployment is normally operated entirely through GitHub Actions. If manual
recovery is required, use the private break-glass access path and activate one
of the retained releases.
