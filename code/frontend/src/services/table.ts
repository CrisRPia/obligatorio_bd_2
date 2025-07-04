import * as backend from "@codegen/backend.api";
import { boardPresidentAuth } from "./auth";
import { toast } from "./toast";
import type { Response } from "./api";

class TableService {
  private headers() {
    return {
      headers: new Headers({
        Authorization: `Bearer ${boardPresidentAuth.getSessionData()?.jwtToken}`,
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
      boardPresidentAuth.logOut();
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

  public async open() {
    const result = await backend.putTableOpen(this.headers());
    return this.handleBooleanReturn(result);
  }

  public async close() {
    const result = await backend.putTableClose(this.headers());
    return this.handleBooleanReturn(result);
  }

  public async search(opts: backend.GetCitizenParams) {
    const result = await backend.getCitizen(opts, this.headers());
    return this.handleStatus(result)?.data.value;
  }

  public async authorize(
    opts: backend.PostTableCitizenIdAuthorizeParams & { citizenId: string },
  ) {
    const result = await backend.postTableCitizenIdAuthorize(
      opts.citizenId,
      opts,
      this.headers(),
    );
    return this.handleBooleanReturn(result);
  }
}

export const table = new TableService();
