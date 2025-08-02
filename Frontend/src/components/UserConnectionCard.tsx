import type {UserConnection} from "../model/snake/lobby";
import type {FC} from "react";

type UserConnectionCardProps = {
    isLobbyLeader: boolean;
    userConnection: UserConnection;
}

const UserConnectionCard: FC<UserConnectionCardProps> = ({ isLobbyLeader, userConnection }) => {
    
    return <div className="relative h-12 m-1">
        <div className={`absolute w-full h-full rounded-xs ${isLobbyLeader ? "bg-yellow-400 dark:opacity-40 opacity-70" : "bg-black dark:opacity-40 opacity-5"} backdrop-blur-2xl`}></div>
        <div className="absolute w-full h-full px-1 py-0.5 grid grid-cols-[1fr_15px]">
            <div>
                <h3 className="text-lg">{userConnection.user.name}</h3>
                <p className="text-xs opacity-40">{userConnection.user.id}</p>
            </div>

            <div className="py-3.5">
                <div className={`w-full h-full rounded-full ${userConnection.active ? "bg-green-400" : "bg-red-400"}`}></div>
            </div>
        </div>
    </div>;
}

export default UserConnectionCard;