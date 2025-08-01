import {type FC, useEffect} from "react";
import {useParams} from "react-router";
import {Connection, type ILobbyClientMethods} from "../util/websocket/Connection.ts";
import type {ChatMessage} from "../model/snake/chatMessage.ts";
import type {User} from "../model/user.ts";
import {delay} from "../util/delay.ts";

const Lobby: FC = () => {
    // const auth = useAppSelector((state) => state.auth);
    const { id } = useParams<{ id: string }>();
    
    const connect = async () => {
        const clientMethods: ILobbyClientMethods = {
            ReceiveMessage: async (chatMessage: ChatMessage)=> {
                console.log("ReceiveMessage", chatMessage);
            },
            SetMessages: async (messages: ChatMessage[])=> {
                console.log("SetMessages", messages);
            },
            UserJoined: async (user: User)=> {
                console.log("UserJoined", user);
            },
            UserLeft: async (user: User)=> {
                console.log("UserLeft", user);
            },
        };

        await Connection.instance.start(clientMethods);
        
        await delay(2000);

        const lobby = await Connection.instance.CreateLobby();
        console.log("lobby", lobby);
        
        await Connection.instance.SendMessage("hello");
    }

    useEffect(() => {
        connect();
    }, []);
    
    return <p>Game: {id}</p>
}

export default Lobby;