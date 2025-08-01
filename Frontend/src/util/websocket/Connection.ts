import {HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {hostUrl} from "../endpoints.ts";
import type {Lobby} from "../../model/snake/lobby.ts";
import type {ChatMessage} from "../../model/snake/chatMessage.ts";
import type {User} from "../../model/user.ts";

export class Connection implements ILobbyServerMethods {
    private conn: HubConnection;
    
    private constructor() {
        this.conn = new HubConnectionBuilder()
            .withUrl(hostUrl() + '/lobby')
            .configureLogging(LogLevel.Information)
            .build();
    }

    CreateLobby(): Promise<Lobby | null> {
        return this.conn.invoke("CreateLobby");
    }
    JoinLobby(lobbyId: string): Promise<Lobby | null> {
        return this.conn.invoke("JoinLobby", lobbyId);
    }
    LeaveCurrentLobby(): Promise<void> {
        return this.conn.invoke("LeaveCurrentLobby");
    }
    SendMessage(content: string): Promise<void> {
        return this.conn.invoke("SendMessage", content);
    }
    
    public async start(clientListener: ILobbyClientMethods) {
        if (this.conn.state == "Disconnected") {
            await this.conn.start();
            (Object.keys(clientListener) as (keyof ILobbyClientMethods)[]).forEach(key => {
                this.conn.on(key, clientListener[key])
            });
        }
    }

    public async stop() {
        await this.conn.stop();
    }
    
    private static ins: Connection|null = null;
    
    public static get instance(): Connection {
        if (Connection.ins == null) {
            Connection.ins = new Connection();
        }
        
        return Connection.ins;
    } 
}

export interface ILobbyServerMethods {
    CreateLobby(): Promise<Lobby|null>;

    JoinLobby(lobbyId: string): Promise<Lobby|null>;

    LeaveCurrentLobby(): Promise<void>;

    SendMessage(content: string): Promise<void>;
}

export interface ILobbyClientMethods {
    ReceiveMessage(chatMessage: ChatMessage): Promise<void>;

    SetMessages(messages: ChatMessage[]): Promise<void>;

    UserJoined(user: User): Promise<void>;

    UserLeft(user: User): Promise<void>;
}