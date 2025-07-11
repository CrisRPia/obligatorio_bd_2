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
public class CitizenController(ICitizenCacheService CitizenCache, IJwtService jwt) : Controller
{
    [HttpPost]
    [SafeAuthorize([Role.Voter])]
    [Route("vote/")]
    public async Task<BooleanReturn> Vote(Ballots votes)
    {
        var tokenData =jwt.GetData(HttpContext);
        var circuitId = tokenData?.CircuitId;

        if (circuitId is null) {
            return new() { Success = false, Message = "Could not get circuit.", Context = new { tokenData }};
        }

        if (CitizenCache.GetCircuitsApprovedCitizen(circuitId) is not (Ulid citizenId, _))
        {
            return new() { Success = false, Message = "No citizen approved to vote." };
        }

        using var connection = DB.NewOpenConnection();
        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var userAssignmentResult = await DB.Queries.SelectUserAssignment(
                new QueriesSql.SelectUserAssignmentArgs { CitizenId = citizenId.ToByteArray() }
            );

            var isObserved = userAssignmentResult
                .Select(
                    (r) =>
                        new CircuitId
                        {
                            EstablishmentId = new Ulid(r.EstablishmentId),
                            CircuitNumber = r.PollingDistrictNumber,
                        }
                )
                .Contains(circuitId);

            var batchCommands = new VoteService
            {
                CitizenId = citizenId,
                Ballots = votes,
                CircuitId = circuitId,
            }.VotesTransaction(isObserved);

            var batch = connection.CreateBatch();
            batch.Transaction = transaction;

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

        CitizenCache.MarkVote(citizenId);
        return BooleanReturn.True;
    }

    [HttpGet]
    [SafeAuthorize([Role.BoardPresident])]
    public async Task<Option<FullCitizen>> GetCitizen(
        [CredencialCivica, Required, FromQuery] string credencialCivica
    )
    {
        var row = await DB.Queries.GetCitizenByCredencialCivica(
            new() { CredencialCivica = credencialCivica }
        );

        if (row is null)
        {
            return new() { };
        }

        return new()
        {
            Value = new()
            {
                UruguayanId = row.UruguayanId,
                Surname = row.Surname,
                Name = row.Name,
                CredencialCivica = row.CredencialCivica,
                BirthDate = DateOnly.FromDateTime(row.Birth),
                CitizenId = new Ulid(row.CitizenId),
            },
        };
    }
}
