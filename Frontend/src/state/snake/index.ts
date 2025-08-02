import {createSlice, type PayloadAction} from "@reduxjs/toolkit";
import type {ChatMessage} from "../../model/snake/chatMessage.ts";
import type {Lobby, UserConnection} from "../../model/snake/lobby.ts";
import type {User} from "../../model/user.ts";

// Define a type for the slice state
export type SnakeState = {
    lobby: Lobby|null;
}

const initialState: SnakeState = {
    lobby: null,
}

const snakeSlice = createSlice({
    name: 'snake',
    initialState,
    reducers: {
        setLobby: (state, action: PayloadAction<Lobby|null>) => {
            console.log("setLobby", action.payload);

            state.lobby = action.payload;
        },
        setUserActive: (state, action: PayloadAction<{userId: string, userActive: boolean}>) => {
            if (state.lobby == null) {
                return;
            }
            
            const userConnection = state.lobby.users[action.payload.userId];
            if (userConnection != null)
            {
                userConnection.active = action.payload.userActive;
            }
        },
        receiveMessage: (state, action: PayloadAction<ChatMessage>) => {
            if (state.lobby == null) {
                return;
            }

            state.lobby.messages.push(action.payload);
        },
        setMessages: (state, action: PayloadAction<ChatMessage[]>) => {
            if (state.lobby == null) {
                return;
            }

            state.lobby.messages = action.payload;
        },
        userJoined: (state, action: PayloadAction<User>) => {
            if (state.lobby == null) {
                return;
            }

            state.lobby.users[action.payload.id] = { user: action.payload, active: true } satisfies UserConnection;
        },
        userLeft: (state, action: PayloadAction<User>) => {
            if (state.lobby == null) {
                return;
            }
            
            delete state.lobby.users[action.payload.id];
        },
    },
});

export const {
    setLobby,
    setUserActive,
    receiveMessage,
    setMessages,
    userJoined,
    userLeft
} = snakeSlice.actions

export default snakeSlice.reducer;