import {type FC, useState} from "react";
import TextInput from "../components/TextInput.tsx";
import {AuthClient} from "../util/client/authClient.ts";
import {useQueryClient} from "@tanstack/react-query";

// This is not an actual route - this page will replace any page if the user is not logged in.
const Login: FC = () => {
    const queryClient = useQueryClient();
    
    const [username, setUsername] = useState<string>("");
    
    const performLogin = async () => {
        const authClient = new AuthClient();
        const userResponse = await authClient.login({
            username: username,
        });
        
        if (userResponse.ok && userResponse.value) {
            await queryClient.invalidateQueries({ queryKey: ['auth'] });
        }
    }
    
    return (
        <form
            className="mt-24 sm:mx-auto mx-2 sm:w-96"
            onSubmit={async e => {
                e.preventDefault();
                await performLogin();
            }}>
            
            <label htmlFor="login-username-input" className="sr-only">Create a username</label>
            <TextInput id="login-username-input" placeholder="create a username here to start playing" onChange={setUsername} value={username} className="w-full rounded-xs" />
            
            {username.length > 2 && (
                <>
                    <br/>
                    <input type="submit" className="w-full hover:cursor-pointer hover:underline" value="Create user"></input>
                </>
            )}
        </form>
    );
}

export default Login;