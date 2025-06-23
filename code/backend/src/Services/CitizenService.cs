using backend.src.Models;
using backend.src.Queries;
using backend.src.Queries.Codegen;
using Isopoh.Cryptography.Argon2;
using MySqlConnector;

namespace backend.src.Services;

public interface ICitizenService
{
    abstract IEnumerable<MySqlBatchCommand> GetCitizen();
    abstract string HashPassword(string password);
    abstract (Ulid id, IEnumerable<MySqlBatchCommand> commands) InsertCitizen(
        FullCitizen citizen,
        string passwordHash
    );
    bool Equals(object? obj);
    bool Equals(CitizenService? other);
    int GetHashCode();
    string ToString();
}

public record CitizenService : ICitizenService
{
    public string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }

    public (Ulid id, IEnumerable<MySqlBatchCommand> commands) InsertCitizen(
        FullCitizen citizen,
        string passwordHash
    )
    {
        var command = new MySqlBatchCommand(QueriesSql.InsertCitizenSql);

        command.Parameters.AddFromObject(
            new QueriesSql.InsertCitizenArgs
            {
                Birth = citizen.BirthDate.ToDateTime(TimeOnly.MinValue),
                CitizenId = citizen.CitizenId.ToByteArray(),
                CredencialCivica = citizen.CredencialCivica,
                Name = citizen.Name,
                PasswordHash = passwordHash,
                UruguayanId = citizen.UruguayanId,
                Surname = citizen.Surname,
            }
        );

        return (citizen.CitizenId, [command]);
    }

    public IEnumerable<MySqlBatchCommand> GetCitizen()
    {
        // TODO
        return [];
    }
}
