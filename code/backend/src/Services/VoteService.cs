using backend.src.Models;
using backend.src.Queries;
using backend.src.Queries.Codegen;
using MySqlConnector;

namespace backend.src.Services;

public record VoteService
{
    public required Ballots Ballots { get; init; }
    public required Ulid CitizenId { get; init; }
    public required CircuitId CircuitId { get; init; }

    public IEnumerable<MySqlBatchCommand> VotesTransaction(bool isObserved)
    {
        var (createdVoteId, createVoteCommand) = CreateVote(isObserved);
        return new[]
        {
            RegisterVoter(),
            [createVoteCommand],
            RegisterBallot(createdVoteId),
        }.SelectMany(x => x);
    }

    private IEnumerable<MySqlBatchCommand> RegisterVoter()
    {
        var parameters = Ballots.Items.Select(
            vote => new QueriesSql.InsertCitizenVoteInPollingDistrictElectionArgs
            {
                CitizenId = CitizenId.ToByteArray(),
                ElectionId = vote.ElectionId.ToByteArray(),
                PollingDistrictNumber = CircuitId.CircuitNumber,
                EstablishmentId = CircuitId.EstablishmentId.ToByteArray(),
            }
        );

        return parameters.Select(p =>
        {
            var command = new MySqlBatchCommand(
                QueriesSql.InsertCitizenVoteInPollingDistrictElectionSql
            );
            command.Parameters.AddFromObject(p);
            return command;
        });
    }

    private static (Ulid rowId, MySqlBatchCommand command) CreateVote(bool isObserved)
    {
        var command = new MySqlBatchCommand(QueriesSql.InsertVoteSql);
        var id = Ulid.NewUlid();

        command.Parameters.AddFromObject(
            new QueriesSql.InsertVoteArgs
            {
                State = isObserved ? VoteState.Valid : VoteState.OutOfDistrict,
                VoteId = id.ToByteArray(),
            }
        );

        return (id, command);
    }

    private IEnumerable<MySqlBatchCommand> RegisterBallot(Ulid voteId)
    {
        return Ballots.Items.Select(vote =>
        {
            var command = new MySqlBatchCommand(QueriesSql.InsertBallotSql);
            command.Parameters.AddFromObject(
                new QueriesSql.InsertBallotArgs
                {
                    BallotId = vote.BallotId.ToByteArray(),
                    VoteId = voteId.ToByteArray(),
                }
            );
            return command;
        });
    }
}
