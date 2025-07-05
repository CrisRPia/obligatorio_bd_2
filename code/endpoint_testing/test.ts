import * as backend from "./backend.api.ts";
import { deepLog, pickRandom } from "./helpers.ts";

function assert(clause: boolean, context: unknown) {
    if (!clause) {
        console.error("Assertion error", context);
        throw -1;
    }
    console.info("Assertion success");
}

function assert_ok(opts: { status: number }) {
    assert(opts.status === 200, opts);
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

    let presidentHeaders = new Headers({
        Authorization: `Bearer ${presidentCredentials.data.jwtToken}`,
    });

    let voterHeaders = new Headers({
        Authorization: `Bearer ${voterCredentials.data.jwtToken}`,
    });

    let openState = await backend.putTableOpen({ headers: presidentHeaders });

    assert_ok(openState);

    for (const voter of response.data.createdCitizens) {
        console.log(`Voting for ${voter.name}`);
        const authorizeResult = await backend.postTableCitizenIdAuthorize(
            voter.citizenId,
            {
                authorizeObserved: true,
            },
            { headers: presidentHeaders },
        );
        assert_ok(authorizeResult);

        let availableElections = (await backend.getElections({
            AvailableForUser: president.citizenId,
        }));

        assert_ok(availableElections);

        for (const election of availableElections.data.items) {
            const voteResult = await backend.postCitizenVote({ items: [
                {
                    ballotId: pickRandom(election.allowedBallots)!.ballotId,
                    electionId: election.electionId,
                }
            ]}, { headers: voterHeaders });

            assert_boolean_return({ ...voteResult, context: voterCredentials.data.jwtToken });
        }
    }

    let closeResult = await backend.putTableClose({ headers: presidentHeaders });

    assert_ok(closeResult);
    deepLog({ closeResult });

    console.log("ElectionId: ", response.data.electionId);
    let electionResult = await backend.postElectionsResult([response.data.electionId])
    assert_ok(closeResult);
    deepLog({ electionResult });
}

main();
