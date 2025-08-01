import {type FC, useState} from "react";
import {Link} from "react-router";
import TextInput from "../components/TextInput.tsx";
import {Connection, type ILobbyClientMethods} from "../util/websocket/Connection.ts";
import type {ChatMessage} from "../model/snake/chatMessage.ts";
import type {User} from "../model/user.ts";
import {delay} from "../util/delay.ts";

const Home: FC = () => {
    const [joinKey, setJoinKey] = useState<string>("");
    
    const createClientMethods = () => {
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
        
        return clientMethods;
    }
    
    const createLobby = async () => {
        const clientMethods: ILobbyClientMethods = createClientMethods();
        await Connection.instance.start(clientMethods);
        while (Connection.instance.status != "Connected")
        {
            console.log("waiting");
            await delay(50);
        }
        
        const lobby = await Connection.instance.CreateLobby();
        console.log("created lobby", lobby);
    }
    
    return (
        <>
            <button onClick={() => createLobby()}>Create Lobby</button>
            
            <br/>

            <TextInput value={joinKey} onChange={text => setJoinKey(text)}/>
            {joinKey && (
                <>
                    <br/>
                    <Link to={`lobby/${joinKey}`}>Join</Link>
                </>
            )}
        </>
    );
}

export default Home;