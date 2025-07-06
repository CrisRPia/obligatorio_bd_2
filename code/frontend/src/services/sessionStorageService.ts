import * as backend from "@codegen/backend.api";

type sessionStorageMap = {
    "presidentAuthData": backend.postAuthTableResponse200["data"],
    "voterAuthData": backend.postAuthVoterResponse200["data"],
    "userData": backend.FullCitizen,
    "departmentCache": backend.Department[],
};

export type SessionStorageKey = keyof sessionStorageMap;

function removeItem(key: keyof sessionStorageMap) {
    sessionStorage.removeItem(key);
}

function set<TKey extends keyof sessionStorageMap>(key: TKey, value: sessionStorageMap[TKey]) {
    sessionStorage.setItem(key, JSON.stringify(value));
}

function get<TKey extends keyof sessionStorageMap>(key: TKey): sessionStorageMap[TKey] | null {
    const output = sessionStorage.getItem(key);
    if (output === null) {
        return null;
    }
    return JSON.parse(output);
}

export const SessionStorage = {
    get,
    set,
    removeItem,
    clear: sessionStorage.clear,
    length: sessionStorage.length,
} as const;
