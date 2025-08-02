import {type FC, useEffect, useRef, useState} from "react";
import {useAppSelector} from "../hooks.ts";
import {Connection} from "../util/websocket/Connection.ts";
import {useNavigate} from "react-router";
import type {UserConnection} from "../model/snake/lobby.ts";
import UserConnectionCard from "./UserConnectionCard.tsx";
import type {SnakeState} from "../state/snake";
import type {AuthState} from "../state/auth";
import ChatMessageCard from "./ChatMessageCard.tsx";
import {playAudio} from "../util/audio/playAudio.ts";

const LobbyHandler: FC = () => {
    const [lastMessageId, setLastMessageId] = useState<string|null>(null);
    const navigate = useNavigate();
    const elementRef = useRef<HTMLOListElement>(null);
    const auth: AuthState = useAppSelector((state) => state.auth);
    const snake: SnakeState = useAppSelector((state) => state.snake);
    
    if (snake.lobby == null) {
        return null;
    } else {
        useEffect(() => {
            const currentMessageId = snake.lobby?.messages[snake.lobby.messages.length-1]?.id ?? null;
            if (lastMessageId != currentMessageId) {
                playAudio("/alert.mp3");
            }
            
            elementRef.current?.scrollTo({
                top: elementRef.current.scrollHeight,
                behavior: 'smooth'
            });
            setLastMessageId(currentMessageId);
        }, [snake.lobby.messages]);
    }
    
    return (
        <div className="grid lg:grid-cols-[1fr_300px] lg:grid-rows-1 grid-cols-1 grid-rows-[1fr_300px] lg:h-[900px] h-[600px] sm:mt-4 mt-0">
            <div className="grid grid-rows-[1fr_30px]">
                <div className="grid grid-rows-[1fr_50px]">
                    <ol className="space-y-2 lg:h-[815px] h-[515px] p-2 overflow-y-scroll" ref={elementRef}>
                        {snake.lobby.messages.map(m => {
                            const isUser = auth.user!.id == m.sender.id
                            return (
                            <li key={m.id} className={`grid ${isUser ? "grid-cols-[1fr_auto]" : "grid-cols-[auto_1fr]"}`}>
                                {isUser && <div></div>}
                                <ChatMessageCard isCurrentUser={isUser} chatMessage={m} />
                                {!isUser && <div></div>}
                            </li>
                            )}
                        )}
                    </ol>
                    
                    <input
                        type="text"
                        className="dark:bg-neutral-800 py-1 px-2 rounded-sm border"
                        onKeyDown={e => {
                            if (e.key == 'Enter') {
                                Connection.instance.sendMessage(e.currentTarget.value);
                                e.currentTarget.value = "";
                            }
                        }}></input>
                </div>
                
                <div className="grid grid-cols-[1fr_85px]">
                    <div>
                        <p className="text-xs mt-2.5 opacity-20">{snake.lobby.id}</p>
                    </div>
                    
                    <div>
                        <button className="bg-blue-300 text-xs" onClick={() => Connection.instance.leaveCurrentLobby().then(() => navigate("/"))}>Leave Lobby</button>
                    </div>
                </div>
            </div>

            <div className="overflow-y-scroll">
                <ul>
                    {Object.values<UserConnection>(snake.lobby.users).map(uc =>
                        <li key={uc.user.id}>
                            <UserConnectionCard isLobbyLeader={(uc.user.id === snake.lobby!.lobbyLeaderId)} userConnection={uc} />
                        </li>)}
                </ul>
            </div>
        </div>
    );
}

export default LobbyHandler;