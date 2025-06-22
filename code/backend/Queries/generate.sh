#!/usr/bin/env bash

script_dir="$(dirname "$0")"
config_path="$script_dir/sqlc.yml"
proyect_root="$script_dir/.."

# Genearte code
sqlc generate -f "$config_path"

# Declare class a partial, since we need to extend it.
# Note the -i '' for macOS compatibility
sed -i  '' 's/public class /public partial class /g' \
    "$script_dir/../src/Queries/Codegen/QueriesSql.cs"

# Declare sql strings public to enable external use of queries.
# Note the -i '' for macOS compatibility
sed -i '' 's/private const string /public const string /g' \
    "$script_dir/../src/Queries/Codegen/QueriesSql.cs"

# Sqlc seems to bug with enums. Hotfix.
sed -i '' 's/Role?/Role/g' \
    "$script_dir/../src/Queries/Codegen/QueriesSql.cs"

sed -i '' 's/Org?/Org/g' \
    "$script_dir/../src/Queries/Codegen/QueriesSql.cs"

sed -i '' 's/Type?/Type/g' \
    "$script_dir/../src/Queries/Codegen/QueriesSql.cs"

# This helper misses a space in the generated code.
sed -i '' 's/@{paramName}/ @{paramName}/g' \
    "$script_dir/../src/Queries/Codegen/Utils.cs"

# Format
dotnet csharpier format "$proyect_root/src/Queries/Codegen"
