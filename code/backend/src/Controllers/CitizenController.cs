using System.ComponentModel.DataAnnotations;
using backend.src.Attributes;
using backend.src.Models;
using backend.src.Queries;
using backend.src.Queries.Codegen;
using backend.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("citizen/")]
public class CitizenController(ICitizenCacheService CitizenCache) : Controller
{
    [HttpPost]
    [Route("{citizenId}/vote/")]
    public async Task<BooleanReturn> Vote(Ulid citizenId, Ballots votes)
    {
        if (CitizenCache.GetCitizenCircuit(citizenId) is not CircuitId circuitId)
        {
            return BooleanReturn.False;
        }

        using var connection = DB.NewOpenConnection();
        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var userAssignmentResult = await DB.Queries.SelectUserAssignment(
                new QueriesSql.SelectUserAssignmentArgs { CitizenId = citizenId.ToByteArray() }
            );

            var isObserved = userAssignmentResult
                .Select((r) => new CircuitId { EstablishmentId = new Ulid(r.EstablishmentId), CircuitNumber = r.PollingDistrictNumber })
                .Contains(circuitId);

            var batchCommands = new VoteService
            {
                CitizenId = citizenId,
                Ballots = votes,
                CircuitId = circuitId,
            }.VotesTransaction(isObserved);

            var batch = connection.CreateBatch();

            foreach (var command in batchCommands)
                batch.BatchCommands.Add(command);

            await batch.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

        return BooleanReturn.True;
    }

    [HttpGet]
    [SafeAuthorize([Role.BoardPresident])]
    public async Task<Option<FullCitizen>> GetCitizen([CredencialCivica, Required, FromQuery] string credencialCivica) {
        var row = await DB.Queries.GetCitizenByCredencialCivica(new() {
                CredencialCivica = credencialCivica
        });

        if (row is null) {
            return new() {};
        }

        return new() {
            Value = new() {
                UruguayanId = row.UruguayanId,
                Surname = row.Surname,
                Name = row.Name,
                CredencialCivica = row.CredencialCivica,
                BirthDate = DateOnly.FromDateTime(row.Birth),
                CitizenId = new Ulid(row.CitizenId),
            }
        };
    }
}
