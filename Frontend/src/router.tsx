import {
    createBrowserRouter,
    type RouteObject,
} from "react-router";
import Home from "./routes/Home.tsx";
import NotFound from "./routes/NotFound.tsx";
import Lobby from "./routes/Lobby.tsx";

const routes: RouteObject[] = [
    {
        path: "",
        element: <Home />,
    },
    {
        path: "lobby",
        element: <Lobby />,
        children: [
            {
                path: ":id",
                element: <Lobby />,
            },
        ]
    },
    {
        path: "*",
        element: <NotFound />
    }
];

export const router = createBrowserRouter(routes, {});