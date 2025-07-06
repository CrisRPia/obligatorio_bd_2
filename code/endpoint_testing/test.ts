import * as backend from "./backend.api.ts";
import { deepLog, pickRandom } from "./helpers.ts";

function assert<T>(clause: boolean, context: T): T {
    if (!clause) {
        console.error("Assertion error", context);
        throw -1;
    }
    console.info("Assertion success");
    return context;
}

function assert_ok<T extends { status: number }>(opts: T): T {
    assert(opts.status === 200, opts);
    return opts;
}

function assert_boolean_return(opts: { status: number, data: backend.BooleanReturn, context?: unknown}) {
    assert_ok(opts);
    assert(opts?.data.success, opts);
}

async function main() {
    // Fake the state. Some aspects of this are random.
    const response = await backend.postDebugFakeInit();
    const president = response.data.tables[0].president;

    // Login as board president
    const presidentCredentials = (
        await backend.postAuthTable({
            credencialCivica: president.credencialCivica,
            password: "pato1234",
            uruguayanId: president.uruguayanId,
        })
    );
    assert_ok(presidentCredentials);

    const voterCredentials = (await backend.postAuthVoter({
        credencialCivica: president.credencialCivica,
        password: "pato1234",
        uruguayanId: president.uruguayanId,
    }));
    assert_ok(voterCredentials);

    let presidentHeaders = { headers: {
        Authorization: `Bearer ${presidentCredentials.data.jwtToken}`,
    }} ;

    deepLog({ voterCredentials });
    let voterHeaders = { headers: {
        Authorization: `Bearer ${voterCredentials.data.jwtToken}`,
    }};

    let openState = await backend.putTableOpen(presidentHeaders);

    assert_ok(openState);

    for (const voter of response.data.createdCitizens) {
        console.log(`Voting for ${voter.name}`);
        const authorizeResult = await backend.postTableCitizenIdAuthorize(
            voter.citizenId,
            {
                authorizeObserved: true,
            },
            presidentHeaders,
        );
        assert_ok(authorizeResult);

        let availableElections = (await backend.getElections({
            "AvailableForCircuit.CircuitNumber": voterCredentials.data.circuit.circuitId.circuitNumber,
            "AvailableForCircuit.EstablishmentId": voterCredentials.data.circuit.circuitId.establishmentId,
        }));

        assert_ok(availableElections);

        for (const election of availableElections.data.items) {
            const voteResult = await backend.postCitizenVote({ items: [
                {
                    ballotId: pickRandom(election.allowedBallots)!.ballotId,
                    electionId: election.electionId,
                }
            ]}, voterHeaders);

            assert_boolean_return({ ...voteResult, context: voterCredentials.data.jwtToken });
        }
    }

    let closeResult = await backend.putTableClose(presidentHeaders);
    assert_ok(closeResult);

    let electionResult = await backend.postElectionsResult(presidentHeaders)
    assert_ok(electionResult);

    deepLog({ electionResult })
    deepLog({ presidentCredentials })
    console.log(JSON.stringify(presidentCredentials.data));
}

main();
