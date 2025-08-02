import {
    createBrowserRouter,
    type RouteObject,
} from "react-router";
import HomeRoute from "./routes/HomeRoute.tsx";
import NotFoundRoute from "./routes/NotFoundRoute.tsx";
import LobbyRoute from "./routes/LobbyRoute.tsx";

const routes: RouteObject[] = [
    {
        path: "",
        element: <HomeRoute />,
    },
    {
        path: "lobby",
        element: <LobbyRoute />,
        children: [
            {
                path: ":id",
                element: <LobbyRoute />,
            },
        ]
    },
    {
        path: "*",
        element: <NotFoundRoute />
    }
];

export const router = createBrowserRouter(routes, {});