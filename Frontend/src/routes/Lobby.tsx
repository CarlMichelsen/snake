import {type FC, useEffect} from "react";
import {useParams} from "react-router";
import {Connection, type ILobbyClientMethods} from "../util/websocket/Connection.ts";
import type {ChatMessage} from "../model/snake/chatMessage.ts";
import type {User} from "../model/user.ts";

const Lobby: FC = () => {
    // const auth = useAppSelector((state) => state.auth);
    const { id } = useParams<{ id: string }>();

    useEffect(() => {
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
        
        Connection.instance.start(clientMethods)
            .then(() => Connection.instance.CreateLobby()
                .then(() => Connection.instance.SendMessage("hello")));
    }, []);
    
    return <p>Game: {id}</p>
}

export default Lobby;