import type { User } from "../../model/user.ts";
import {BaseClient} from "../baseClient.ts";
import type {Login} from "../../model/auth/login.ts";
import type {ServiceResponse} from "../../model/serviceResponse.ts";

export class AuthClient extends BaseClient {
    public async login(login: Login): Promise<ServiceResponse<User>> {
        return await this.request("POST", "api/v1/auth/login", login);
    }

    public async logout(): Promise<ServiceResponse<void>> {
        return await this.request("DELETE", "api/v1/auth/logout");
    }

    public async user(): Promise<ServiceResponse<User>> {
        return await this.request("GET", "api/v1/auth/user");
    }
}