import type { ElectionType } from "@codegen/backend.api";

export function translateElection(type: ElectionType){
    switch (type) {
        case "MunicipalElection":
            return "Elección Municipal"
        case "Plebiscite":
            return "Plebiscito"
        case "Presidential":
            return "Elección Presidencial"
        case "Referendum":
            return "Referéndum"
        case "Runoff":
            return "Ballotage"
        default:
            break;
    }
}