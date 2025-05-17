// eslint-disable-next-line @typescript-eslint/no-redeclare
export const VoteType = {
    List: "List",
    Boolean: "Boolean",
    Count: "Count",
};
// eslint-disable-next-line @typescript-eslint/no-redeclare
export const Role = {
    Voter: "Voter",
    BoardPresident: "BoardPresident",
    Admin: "Admin",
    Police: "Police",
};
// eslint-disable-next-line @typescript-eslint/no-redeclare
export const PollingStationMemberRole = {
    President: "President",
    Secretary: "Secretary",
    Vocal: "Vocal",
};
// eslint-disable-next-line @typescript-eslint/no-redeclare
export const ElectionType = {
    Presidential: "Presidential",
    Referendum: "Referendum",
    Plebiscite: "Plebiscite",
    MunicipalElection: "MunicipalElection",
    Runoff: "Runoff",
};
// eslint-disable-next-line @typescript-eslint/no-redeclare
export const ElectionState = {
    Open: "Open",
    Closed: "Closed",
};
export const getPostAuthPollingStationLoginUrl = () => {
    return `/auth/polling_station/login`;
};
export const postAuthPollingStationLogin = async (baseCitizen, options) => {
    const res = await fetch(getPostAuthPollingStationLoginUrl(), {
        ...options,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(baseCitizen),
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getPostAuthPollingStationRegisterUrl = () => {
    return `/auth/polling_station/register`;
};
export const postAuthPollingStationRegister = async (pollingStationMember, options) => {
    const res = await fetch(getPostAuthPollingStationRegisterUrl(), {
        ...options,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(pollingStationMember),
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getPostAuthPoliceOfficerRegisterUrl = () => {
    return `/auth/police_officer/register`;
};
export const postAuthPoliceOfficerRegister = async (pollingStationMember, options) => {
    const res = await fetch(getPostAuthPoliceOfficerRegisterUrl(), {
        ...options,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(pollingStationMember),
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getPostAuthPoliceOfficerLoginUrl = () => {
    return `/auth/police_officer/login`;
};
export const postAuthPoliceOfficerLogin = async (pollingStationMember, options) => {
    const res = await fetch(getPostAuthPoliceOfficerLoginUrl(), {
        ...options,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(pollingStationMember),
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getGetDepartmentsDepartmentIdCircuitsCircuitIdResultsElectionIdUrl = (departmentId, circuitId, electionId) => {
    return `/departments/${departmentId}/circuits/${circuitId}/results/${electionId}`;
};
export const getDepartmentsDepartmentIdCircuitsCircuitIdResultsElectionId = async (departmentId, circuitId, electionId, options) => {
    const res = await fetch(getGetDepartmentsDepartmentIdCircuitsCircuitIdResultsElectionIdUrl(departmentId, circuitId, electionId), {
        ...options,
        method: "GET",
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getPutDepartmentsDepartmentIdCircuitsCircuitIdAuthorizeVoteVoteIdUrl = (departmentId, circuitId, voteId, params) => {
    const normalizedParams = new URLSearchParams();
    Object.entries(params || {}).forEach(([key, value]) => {
        if (value !== undefined) {
            normalizedParams.append(key, value === null ? "null" : value.toString());
        }
    });
    return normalizedParams.size
        ? `/departments/${departmentId}/circuits/${circuitId}/authorize_vote/${voteId}?${normalizedParams.toString()}`
        : `/departments/${departmentId}/circuits/${circuitId}/authorize_vote/${voteId}`;
};
export const putDepartmentsDepartmentIdCircuitsCircuitIdAuthorizeVoteVoteId = async (departmentId, circuitId, voteId, params, options) => {
    const res = await fetch(getPutDepartmentsDepartmentIdCircuitsCircuitIdAuthorizeVoteVoteIdUrl(departmentId, circuitId, voteId, params), {
        ...options,
        method: "PUT",
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getGetDepartmentsDepartmentIdCircuitsUrl = (departmentId) => {
    return `/departments/${departmentId}/circuits`;
};
export const getDepartmentsDepartmentIdCircuits = async (departmentId, options) => {
    const res = await fetch(getGetDepartmentsDepartmentIdCircuitsUrl(departmentId), {
        ...options,
        method: "GET",
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getPostCitizenCitizenIdVoteCircuitIdUrl = (citizenId, circuitId) => {
    return `/citizen/${citizenId}/vote/${circuitId}`;
};
export const postCitizenCitizenIdVoteCircuitId = async (citizenId, circuitId, incomingVotes, options) => {
    const res = await fetch(getPostCitizenCitizenIdVoteCircuitIdUrl(citizenId, circuitId), {
        ...options,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(incomingVotes),
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getPostDebugFakeUrl = () => {
    return `/debug/Fake`;
};
export const postDebugFake = async (fakeInput, options) => {
    const res = await fetch(getPostDebugFakeUrl(), {
        ...options,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(fakeInput),
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getGetDebugJWTUrl = () => {
    return `/debug/JWT`;
};
export const getDebugJWT = async (options) => {
    const res = await fetch(getGetDebugJWTUrl(), {
        ...options,
        method: "GET",
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getPostDebugJWTUrl = () => {
    return `/debug/JWT`;
};
export const postDebugJWT = async (role, options) => {
    const res = await fetch(getPostDebugJWTUrl(), {
        ...options,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(role),
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getGetDebugTestAuthorsUrl = () => {
    return `/debug/TestAuthors`;
};
export const getDebugTestAuthors = async (options) => {
    const res = await fetch(getGetDebugTestAuthorsUrl(), {
        ...options,
        method: "GET",
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getPostDebugTestAuthorsUrl = () => {
    return `/debug/TestAuthors`;
};
export const postDebugTestAuthors = async (createAuthorArgs, options) => {
    const res = await fetch(getPostDebugTestAuthorsUrl(), {
        ...options,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(createAuthorArgs),
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getGetDepartmentsUrl = () => {
    return `/departments`;
};
export const getDepartments = async (options) => {
    const res = await fetch(getGetDepartmentsUrl(), {
        ...options,
        method: "GET",
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getGetElectionsUrl = (params) => {
    const normalizedParams = new URLSearchParams();
    Object.entries(params || {}).forEach(([key, value]) => {
        if (value !== undefined) {
            normalizedParams.append(key, value === null ? "null" : value.toString());
        }
    });
    return normalizedParams.size
        ? `/elections?${normalizedParams.toString()}`
        : `/elections`;
};
export const getElections = async (params, options) => {
    const res = await fetch(getGetElectionsUrl(params), {
        ...options,
        method: "GET",
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getPostElectionsUrl = () => {
    return `/elections`;
};
export const postElections = async (election, options) => {
    const res = await fetch(getPostElectionsUrl(), {
        ...options,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(election),
    });
    const data = await res.json();
    return { status: res.status, data };
};
export const getGetElectionsElectionIdUrl = (electionId) => {
    return `/elections/${electionId}`;
};
export const getElectionsElectionId = async (electionId, options) => {
    const res = await fetch(getGetElectionsElectionIdUrl(electionId), {
        ...options,
        method: "GET",
    });
    const data = await res.json();
    return { status: res.status, data };
};
