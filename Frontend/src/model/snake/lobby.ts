import type {User} from "../user.ts";
import type {ChatMessage} from "./chatMessage.ts";

export type Lobby = {
    Id: string,
    LobbyLeader: User,
    Messages: ChatMessage[],
    Users: { [key: string]: User }
}