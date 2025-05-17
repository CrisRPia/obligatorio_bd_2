import { ConfigExternal } from "orval";

export default {
    backend: {
        input: {
            target: "temp/backend.schema.json",
        },
        output: {
            target: "temp/backend.api.ts",
            prettier: true,
            client: "fetch",
        },
    },
} satisfies ConfigExternal;

