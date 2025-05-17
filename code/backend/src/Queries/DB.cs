using backend.src.Queries.Codegen;

namespace backend.src.Queries;

public static class DB {
    public static string CreateConnectionString(string dbHost, string dbPort, string dbName, string dbUser, string dbPassword) {
        return $"Server={dbHost};Port={dbPort};Database={dbName};Uid={dbUser};Pwd={dbPassword};";
    }

    public static string CreateConnectionStringFromEnv() {
        static string forgor(string envPath)
        {
            var output = Environment.GetEnvironmentVariable(envPath);
            if (output is not string verifiedOutput)
            {
                Console.WriteLine();
                throw new InvalidOperationException($"You must set the {envPath} environment variable.");
            }

            return verifiedOutput;
        }

        return CreateConnectionString(
            dbHost: forgor("DB_HOST"),
            dbPort: forgor("DB_PORT"),
            dbName: forgor("DB_NAME"),
            dbUser: forgor("DB_USER"),
            dbPassword: forgor("DB_PASSWORD")
        );
    }

    private static QueriesSql CreateQueryInstance() {
        return new QueriesSql(CreateConnectionStringFromEnv());
    }
    public static QueriesSql Queries { get; } = CreateQueryInstance();
}
