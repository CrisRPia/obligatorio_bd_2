import * as backend from "./backend.api.ts";
import { deepLog, pickRandom } from "./helpers.ts";

async function main() {
    // Fake the state. Some aspects of this are random.
    const response = await backend.postDebugFakeInit();
    const president = response.data.tables[0].president;

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

    let openState = await backend.putTableOpen({ headers: presidentHeaders });

    deepLog({ openState });

    for (const voter of response.data.createdCitizens) {
        console.log(`Voting for ${voter.name}`);
        await backend.postTableCitizenIdAuthorize(
            voter.citizenId,
            {
                authorizeObserved: true,
            },
            { headers: presidentHeaders },
        );

        let availableElections = (await backend.getElections({
            AvailableForUser: president.citizenId,
        })).data;

        for (const election of availableElections.items) {
            await backend.postCitizenCitizenIdVote(voter.citizenId, { items: [
                {
                    ballotId: pickRandom(election.allowedBallots)!.ballotId,
                    electionId: election.electionId,
                }
            ]});
        }
    }

    let closeResult = await backend.putTableClose({ headers: presidentHeaders });
    deepLog({ closeResult });

    console.log("ElectionId: ", response.data.electionId);
    let electionResult = await backend.postElectionsResult([response.data.electionId])
    deepLog({ electionResult });
}

main();
