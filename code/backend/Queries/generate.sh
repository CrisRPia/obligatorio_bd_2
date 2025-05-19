#!/usr/bin/env bash

script_dir="$(dirname "$0")"
config_path="$script_dir/sqlc.yml"
proyect_root="$script_dir/.."

# Genearte code
sqlc generate -f "$config_path"

# Declare class a partial, since we need to extend it.
sed -i 's/public class /public partial class /g' \
    "$script_dir/../src/Queries/Codegen/QueriesSql.cs"

# Declare sql strings public to enable external use of queries.
sed -i 's/private const string /public const string /g' \
    "$script_dir/../src/Queries/Codegen/QueriesSql.cs"

# Format
dotnet csharpier format "$proyect_root"
