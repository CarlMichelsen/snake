import {type FC} from "react";
import {Connection} from "../util/websocket/Connection.ts";
import {useAppDispatch, useOnce, useSignalRConnection} from "../hooks.ts";
import {setLobby} from "../state/snake";
import {delay} from "../util/delay.ts";
import {useNavigate, useParams} from "react-router";
import LobbyHandler from "../components/LobbyHandler.tsx";
import {HubConnectionState} from "@microsoft/signalr";

const LobbyRoute: FC = () => {
    const dispatch = useAppDispatch()
    const navigate = useNavigate();
    const { connectionState } = useSignalRConnection();
    const { id } = useParams<{ id: string }>();
    
    const ensureLobby = async () => {
        console.log("Attempting to connect to", id ?? "new lobby");
        
        while (Connection.instance.connection.state != HubConnectionState.Connected) {
            await delay(100);
        }
        
        if (id != null) {
            try {
                const lobby = await Connection.instance.joinLobby(id);
                if (lobby) {
                    navigate(`/lobby/${lobby.id}`, { replace: true });
                } else {
                    navigate("/");
                }
                
                dispatch(setLobby(lobby));
            } catch (e) {
                navigate("/");
            }
        } else {
            const lobby = await Connection.instance.createLobby();
            dispatch(setLobby(lobby));
            
            if (lobby) {
                navigate(`/lobby/${lobby.id}`, { replace: true });
            } else {
                navigate("/");
            }
        }
    }

    useOnce(() => ensureLobby());
    
    return connectionState == HubConnectionState.Connected
        ? <LobbyHandler />
        : <p>{connectionState}</p>
}

export default LobbyRoute;