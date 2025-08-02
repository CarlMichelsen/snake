import type {User} from "../user.ts";
import type {ChatMessage} from "./chatMessage.ts";

export type Lobby = {
    id: string,
    lobbyLeaderId: string,
    messages: ChatMessage[],
    users: { [key: string]: UserConnection }
}

export type UserConnection = {
    user: User,
    active: boolean,
}