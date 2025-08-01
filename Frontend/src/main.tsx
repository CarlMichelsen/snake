import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import {Provider} from "react-redux";
import {RouterProvider} from "react-router";
import {router} from "./router.tsx";
import {store} from "./state/store.ts";

const queryClient = new QueryClient()

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <QueryClientProvider client={queryClient}>
            <Provider store={store}>
                <App>
                    <RouterProvider router={router} />
                </App>
            </Provider>
        </QueryClientProvider>
    </StrictMode>,
)
