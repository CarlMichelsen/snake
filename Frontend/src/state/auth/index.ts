import {createSlice, type PayloadAction} from '@reduxjs/toolkit'
import {type User} from "../../model/user.ts";

// Define a type for the slice state
type AuthState = {
    status: "pending"|"loggedIn"|"loggedOut";
    user: User|null;
}

const initialState: AuthState = {
    status: "pending",
    user: null,
}

const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        login: (state, action: PayloadAction<User>) => {
            if (action.payload == null) {
                return;
            }

            state.status = "loggedIn";
            state.user = action.payload;
        },
        logout: (state) => {
            state.status = "loggedOut";
            state.user = null;
        },
    },
});

export const { login, logout } = authSlice.actions

export default authSlice.reducer;