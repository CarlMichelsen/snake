import {generateTraceContext} from "./tracecontext.ts";
import {hostUrl} from "./endpoints.ts";
import type {ServiceResponse} from "../model/serviceResponse.ts";

type HttpMethod = "GET"
    | "HEAD"
    | "POST"
    | "PUT"
    | "DELETE"
    | "CONNECT"
    | "OPTIONS"
    | "TRACE"
    | "PATCH";

export abstract class BaseClient {
    protected async request<TResponse>(method: HttpMethod, path: string, body: object|null = null, headers?: { [key: string]: string }): Promise<ServiceResponse<TResponse>> {
        const traceContext = generateTraceContext(path);
        const init: RequestInit = {
            method: method,
            credentials: 'include',
            headers: {
                "Content-Type": "application/json",
                "traceparent": traceContext.traceparent,
                ...headers },
        };

        if (body) {
            init.body = JSON.stringify(body);
        }

        const response = await fetch(hostUrl() + BaseClient.ensureLeadingSlash(path), init);
        const json = await response.json();
        if (json != null)
        {
            return json as ServiceResponse<TResponse>;
        }

        throw new Error(`Failed to fetch [${method}] - ${path}`);
    }

    private static ensureLeadingSlash(input: string): string {
        if (!input.startsWith("/")) {
            return "/" + input;
        }

        return input;
    }
} 