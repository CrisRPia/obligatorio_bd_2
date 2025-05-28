using backend.src.Models;
using backend.src.Queries;
using backend.src.Queries.Codegen;
using MySqlConnector;

namespace backend.src.Services;

public record CitizenService
{
    public static (Ulid id, IEnumerable<MySqlBatchCommand> commands) InsertCitizen(FullCitizen citizen, string passwordHash) {
        var command = new MySqlBatchCommand(QueriesSql.InsertCitizenSql);
        var id = Ulid.NewUlid();

        command.Parameters.AddFromObject(new QueriesSql.InsertCitizenArgs {
                Birth = citizen.BirthDate.ToDateTime(TimeOnly.MinValue),
                CitizenId = id.ToByteArray(),
                CredencialCivica = citizen.CredencialCivica,
                Name = citizen.Name,
                PasswordHash = passwordHash,
                UruguayanId = citizen.UruguayanId,
                Surname = citizen.Surname,
        });
        
        return (id, [command]);
    }
}
