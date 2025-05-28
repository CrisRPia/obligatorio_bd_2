using backend.src.Attributes;
using backend.src.Models;
using backend.src.Queries;
using backend.src.Queries.Codegen;
using backend.src.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("citizen/")]
public class CitizenController : Controller
{
    [HttpPost]
    [SafeAuthorize(roles: [Role.Voter])]
    [Route("{citizenId}/vote/{circuitId}/")]
    public async Task<DefaultOk> Vote(
        Ulid citizenId,
        Ulid circuitId,
        Ballots votes
    )
    {
        using var connection = DB.NewConnection();
        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var userAssignmentResult = await DB.Queries.SelectUserAssignment(
                new QueriesSql.SelectUserAssignmentArgs
                {
                    CitizenId = citizenId.ToByteArray(),
                }
            );

            var isObserved = userAssignmentResult
                .Select((r) => Ulid.Parse(r.PollingDistrictNumber))
                .Contains(circuitId);

            // TODO: ADD CHECK FOR BALLOTS

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

        return DefaultOk.Instance;
    }
}
