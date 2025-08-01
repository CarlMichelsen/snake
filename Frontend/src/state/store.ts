import { configureStore } from '@reduxjs/toolkit'
import authReducer from "./auth";

const customLoggerMiddleware = (storeAPI: { getState: () => RootState }) => (next: (action: any) => any) => (action: any) => {
    if (import.meta.env.VITE_APP_ENV !== 'development') {
        return next(action);
    }

    const result = next(action);
    console.log(action, '->', storeAPI.getState());
    return result;
};

export const store = configureStore({
    reducer: {
        auth: authReducer,
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(customLoggerMiddleware),
})

export type RootState = ReturnType<typeof store.getState>

export type AppDispatch = typeof store.dispatch