import type {User} from "../user.ts";

export type ChatMessage = {
    id: string,
    sender: User,
    content: string,
    timeStamp: string,
}