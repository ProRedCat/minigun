#!/usr/bin/env bash

set -Eeuo pipefail

systemctl try-restart minigun.service
