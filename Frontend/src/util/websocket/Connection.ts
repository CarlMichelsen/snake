import {HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {hostUrl} from "../endpoints.ts";
import type {Lobby} from "../../model/snake/lobby.ts";
import type {ChatMessage} from "../../model/snake/chatMessage.ts";
import type {User} from "../../model/user.ts";

export class Connection implements ILobbyServerMethods {
    private readonly conn: HubConnection;
    
    private constructor() {
        this.conn = new HubConnectionBuilder()
            .withUrl(hostUrl() + '/lobby')
            .configureLogging(LogLevel.Information)
            .build();
    }
    
    public get connection() {
        return this.conn;
    }
    
    createLobby(): Promise<Lobby | null> {
        return this.conn.invoke("CreateLobby");
    }
    joinLobby(lobbyId: string): Promise<Lobby | null> {
        return this.conn.invoke("JoinLobby", lobbyId);
    }
    leaveCurrentLobby(): Promise<void> {
        return this.conn.invoke("LeaveCurrentLobby");
    }
    sendMessage(content: string): Promise<void> {
        return this.conn.invoke("SendMessage", content);
    }
    
    public async start(clientListener: ILobbyClientMethods) {
        const titleCase = (str: string): string =>
            str.split(' ').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');
        
        if (this.conn.state == "Disconnected") {
            await this.conn.start();
            (Object.keys(clientListener) as (keyof ILobbyClientMethods)[]).forEach(key => {
                this.conn.on(titleCase(key), clientListener[key])
            });
        }
    }

    public stop() {
        this.conn.stop();
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
    createLobby(): Promise<Lobby|null>;

    joinLobby(lobbyId: string): Promise<Lobby|null>;

    leaveCurrentLobby(): Promise<void>;

    sendMessage(content: string): Promise<void>;
}

export interface ILobbyClientMethods {
    setLobby(lobby: Lobby): void;

    userActive(userId: string, userActive: boolean): void;
    
    receiveMessage(chatMessage: ChatMessage): void;

    setMessages(messages: ChatMessage[]): void;

    userJoined(user: User): void;

    userLeft(user: User): void;
}