import {useAppDispatch, useAppSelector} from "./hooks.ts";
import {type FC, type ReactNode, useEffect} from "react";
import {useQuery} from "@tanstack/react-query";
import {AuthClient} from "./util/client/authClient.ts";
import {login, logout} from "./state/auth";
import Login from "./routes/Login.tsx";


type AppProps = {
  children: ReactNode;
}

const App: FC<AppProps> = ({ children }) => {
    const auth = useAppSelector((state) => state.auth);
    const dispatch = useAppDispatch()
    
    const query = useQuery({
        queryKey: ['auth'],
        queryFn: async () => {
            const authClient = new AuthClient();
            return  await authClient.user();
        },
        staleTime: 1000 * 60 * 10,
    });

    useEffect(() => {
        if (query.status === "success") {
            if (query.data.ok && query.data.value) {
                dispatch(login(query.data.value));
            } else {
                dispatch(logout());
            }
        }
    }, [query]);

    switch (auth.status) {
        case "loggedIn":
            return <div className="container mx-auto">{children}</div>;
        case "loggedOut":
            return <div className="container mx-auto"><Login /></div>;
        case "pending":
            return null;
        default:
            throw new Error("Invalid auth status");
    }
}

export default App
