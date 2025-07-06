import * as backend from "@codegen/backend.api";
import { SessionStorage, type SessionStorageKey } from "./sessionStorageService";

// This is ugly AF. I know. I wrote it. -- CR
export const supportedUsers = [
    "Voter",
    "BoardPresident"
] as const satisfies backend.Role[];
export type SupportedUser = (typeof supportedUsers)[number]

const authMethodMap = {
    "BoardPresident": backend.postAuthTable,
    "Voter": backend.postAuthVoter, // TODO
} as const satisfies Record<SupportedUser, unknown>;
type AuthMethodMap = typeof authMethodMap;

const authSessionStorageMap = {
    "BoardPresident": "presidentAuthData",
    "Voter": "voterAuthData",
} as const satisfies Record<SupportedUser, SessionStorageKey>;

type AuthData<T extends SupportedUser> = Awaited<ReturnType<AuthMethodMap[T]>>["data"];
type AuthArgs<T extends SupportedUser> = Parameters<AuthMethodMap[T]>[0]

// All of that to avoid writing this twice.
// Was it worth it? No. Did I have fun? Somewhat. -- CR
class Auth<TUserType extends SupportedUser> {
    private loginType: TUserType;
    constructor(loginType: TUserType) {
        this.loginType = loginType;
    }

    public getAuthHeaders() {
        const token = this.getSessionData()?.jwtToken;
        return { Authorization: `Bearer ${token}` }
    }

    public async logIn(opts: AuthArgs<TUserType>): Promise<AuthData<TUserType> | undefined> {
        const { data, headers, status } = await authMethodMap[this.loginType](opts);

        if (status !== 200) {
            console.error("Error while authenticating.", { data, headers, status })
            return undefined;
        }

        // FIXME: as any
        SessionStorage.set(authSessionStorageMap[this.loginType], data as any);

        return data;
    }

    public getSessionData() {
        return SessionStorage.get(authSessionStorageMap[this.loginType]);
    }

    public isLoggedIn() {
        return !!this.getSessionData();
    }

    public logOut(): never {
        SessionStorage.clear();
        window.location.href = "/"
        throw "We must never get here";
    }
}

export const boardPresidentAuth = new Auth("BoardPresident");
export const voterAuth = new Auth("Voter");
