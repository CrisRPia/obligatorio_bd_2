import * as backend from "@codegen/backend.api";

import type { Response } from "./api";
import { voterAuth } from "./auth";
import { toast } from "./toast";
import { SessionStorage } from "./sessionStorageService";

class VoterService {
  private getHeaders() {
    return {
      headers: new Headers({
        Authorization: `Bearer ${voterAuth.getSessionData()?.jwtToken}`,
      }),
    };
  }

  private logError(opts: unknown) {
    console.error("Error fetching data", opts);
  }

  private toastError(opts: unknown) {
    this.logError(opts);
    toast("Ocurrió un error al realizar la acción");
  }

  private handleStatus<T extends Response<unknown>>(opts: T): T | undefined {
    if (opts.status === 401) {
      toast("Tu sesión a expirado. Redirigiendo...");
      voterAuth.logOut();
    }

    if (opts.status < 400) {
      return opts;
    }

    this.logError(opts);
    return undefined;
  }

  private handleBooleanReturn(opts: Response<backend.BooleanReturn>) {
    const output = this.handleStatus(opts);
    if (!output) {
      return output;
    }

    if (!output.data.success) {
      this.toastError(opts);
      return undefined;
    }

    return output;
  }

  public async getOpenElections() {
    const user = voterAuth.getSessionData()?.circuit.building.zone.locality.department.departmentId;
    const result = await backend.getElections({
      OnlyOpenOrClosed: "Open",
      DepartmentId: user,
    }, this.getHeaders());

    return this.handleStatus(result);
  }

  public async vote(ballots: backend.Ballots) {
    const result = await backend.postCitizenVote(ballots, this.getHeaders());

    return this.handleBooleanReturn(result);
  }

  public async getDepartments() {
    const cache = SessionStorage.get("departmentCache");

    if (cache !== null) {
      return cache;
    }

    const result = this.handleStatus(await backend.getDepartments(this.getHeaders()));

    if (result?.data !== undefined) {
      SessionStorage.set("departmentCache", result.data);
    }

    return result?.data;
  }
}

export const voter = new VoterService();
