import { ConfigExternal } from "orval";

export default {
    backend: {
        input: {
            target: "temp/backend.schema.json",
        },
        output: {
            baseUrl: "http://localhost:8080/",
            target: "temp/backend.api.ts",
            prettier: true,
            client: "fetch",
        },
    },
} satisfies ConfigExternal;

