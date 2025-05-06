FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /voting_backend_db2
COPY *.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o out
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /voting_backend_db2
COPY --from=build /voting_backend_db2/out .

ENTRYPOINT ["dotnet", "backend.dll"]
