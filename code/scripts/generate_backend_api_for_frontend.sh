#!/usr/bin/env bash

# Get schema
curl http://localhost:8080/swagger/v1/swagger.json \
    > ./frontend_codegen/temp/backend.schema.json

# Generate frontend client
orval --config ./frontend_codegen/orval.config.ts

# Compile to js
tsc -p ./frontend_codegen/temp/

# Move compiled code to frontend project
mv ./frontend_codegen/temp/*.js \
    ./frontend_codegen/temp/*.d.ts \
    ../frontend/codegen/

# Remove leftovers
rm ./frontend_codegen/temp/*.ts
