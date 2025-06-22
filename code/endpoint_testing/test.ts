import * as backend from "./backend.api.ts";

async function main() {
    // Fake the state. Some aspects of this are random.
    const { data, status, headers } = await backend.postDebugFakeInit();
    const president = data.tables[0].president;

    // Login as board president
    const presidentCredentials = (
        await backend.postAuth({
            credencialCivica: president.credencialCivica,
            password: "pato1234",
            uruguayanId: president.uruguayanId,
        })
    ).data;

    let presidentHeaders = new Headers({
        Authorization: `Bearer ${presidentCredentials.jwtToken}`,
    });

    let authorizationResult = await backend.postTableCitizenIdAuthorize(
        president.citizenId,
        {
            authorizeObserved: true,
        },
        { headers: presidentHeaders },
    );

    console.dir(authorizationResult, { depth: null });

    let availableElections = (await backend.getElections({
        AvailableForUser: president.citizenId,
    })).data;

    // console.dir(availableElections, { depth: null });

    for (const election of availableElections.items) {
        let voteResult = await backend.postCitizenCitizenIdVote(president.citizenId, { items: [
            {
                ballotId: election.allowedBallots[0].ballotId,
                electionId: election.electionId,
            }
        ]});

        console.dir(voteResult, { depth: null });
    }

    let closeResult = await backend.putTableClose({ headers: presidentHeaders });

    console.dir(closeResult, { depth: null });

    // TODO: GET RESULTS
}

main();
