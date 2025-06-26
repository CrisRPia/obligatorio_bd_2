import * as backend from "@codegen/backend.api";

type sessionStorageMap = {
    "authToken": string,
    "userType": string,
    "userData": backend.FullCitizen,
    "credencialCivica": string
};

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
} as const;