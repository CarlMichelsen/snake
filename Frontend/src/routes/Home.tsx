import {type FC, useState} from "react";
import {Link} from "react-router";
import TextInput from "../components/TextInput.tsx";

const Home: FC = () => {
    const [joinKey, setJoinKey] = useState<string>("");
    
    return (
        <>
            <Link to="lobby">Create Lobby</Link>
            
            <br/>

            <TextInput value={joinKey} onChange={text => setJoinKey(text)}/>
            {joinKey && (
                <>
                    <br/>
                    <Link to={`lobby/${joinKey}`}>Join</Link>
                </>
            )}
        </>
    );
}

export default Home;