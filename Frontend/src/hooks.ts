import {useDispatch, useSelector} from 'react-redux'
import type { RootState, AppDispatch } from './state/store'

// Use throughout your app instead of plain `useDispatch` and `useSelector`
export const useAppDispatch = useDispatch.withTypes<AppDispatch>()
export const useAppSelector = useSelector.withTypes<RootState>()

import {useState, useEffect, useRef} from 'react';
import {Connection, type ILobbyClientMethods} from "./util/websocket/Connection.ts";
import {HubConnectionState} from "@microsoft/signalr";
import type {ChatMessage} from "./model/snake/chatMessage.ts";
import {receiveMessage, setLobby, setMessages, setUserActive, userJoined, userLeft} from "./state/snake";
import type {User} from "./model/user.ts";
import type {Lobby} from "./model/snake/lobby.ts";

export const useDarkMode = () => {
    const [isDarkMode, setIsDarkMode] = useState(false);

    useEffect(() => {
        const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
        setIsDarkMode(mediaQuery.matches);

        const handleChange = (event: MediaQueryListEvent) =>
            setIsDarkMode(event.matches);

        mediaQuery.addEventListener('change', handleChange);
        return () => mediaQuery.removeEventListener('change', handleChange);
    }, []);

    return isDarkMode;
}

export const useOnce = (onMount: () => void) => {
    const hasRun = useRef(false);

    useEffect(() => {
        if (!hasRun.current) {
            hasRun.current = true;
            onMount();
        }
    }, []);
}

export const useSignalRConnection = () => {
    const dispatch = useAppDispatch()
    const [connectionState, setConnectionState] = useState<HubConnectionState>(HubConnectionState.Disconnected);

    const clientMethods: ILobbyClientMethods = {
        setLobby: (lobby: Lobby)=> { dispatch(setLobby(lobby)); },
        userActive: (userId: string, userActive: boolean)=> { dispatch(setUserActive({userId, userActive})); },
        receiveMessage: (chatMessage: ChatMessage)=> { dispatch(receiveMessage(chatMessage)); },
        setMessages: (messages: ChatMessage[])=> { dispatch(setMessages(messages)); },
        userJoined: (user: User)=> { dispatch(userJoined(user)); },
        userLeft: (user: User)=> { dispatch(userLeft(user)); },
    };

    const updateState = () => {
        console.log("ConnectionState", Connection.instance.connection.state);
        setConnectionState(Connection.instance.connection.state);
    }

    useOnce(() => {
        Connection.instance.start(clientMethods).then(() => {
            updateState()
            Connection.instance.connection.onreconnecting(updateState);
            Connection.instance.connection.onreconnected(updateState);
            Connection.instance.connection.onclose(updateState);
        });
    });

    return { connectionState }
}