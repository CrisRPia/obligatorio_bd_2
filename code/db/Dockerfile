# Use the official MySQL long-term support image as the base
FROM mysql:lts

# Expose the standard MySQL port
EXPOSE 3306

# Copy the initialization script into the directory
# where the official MySQL image looks for initialization scripts.
# These scripts are executed when the container is first created and the database is initialized.
COPY ./init/ddl.sql /docker-entrypoint-initdb.d/

# The official MySQL image has a default ENTRYPOINT that handles database initialization
# and starting the server, including running scripts from /docker-entrypoint-initdb.d/.
# We don't need to specify an ENTRYPOINT here unless we want to override the default behavior,
# which is not required for this use case.
