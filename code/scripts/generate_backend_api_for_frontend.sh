#!/usr/bin/env bash

# Get schema
curl http://localhost:8080/swagger/v1/swagger.json \
    > ./frontend_codegen/temp/backend.schema.json

# Generate frontend client
orval --config ./frontend_codegen/orval.config.ts

# Move compiled code to where it's needed
cp ./frontend_codegen/temp/backend.api.ts ../endpoint_testing/

# Remove leftovers
rm ./frontend_codegen/temp/*.ts
