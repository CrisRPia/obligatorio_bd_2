#!/usr/bin/env bash

script_dir="$(dirname "$0")"
config_path="$script_dir/sqlc.yml"
proyect_root="$script_dir/.."

sqlc generate -f "$config_path"
dotnet csharpier format "$proyect_root"
