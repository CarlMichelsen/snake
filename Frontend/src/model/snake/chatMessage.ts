import type {User} from "../user.ts";

export type ChatMessage = {
    Id: string,
    Sender: User,
    Content: string,
    TimeStamp: string,
}