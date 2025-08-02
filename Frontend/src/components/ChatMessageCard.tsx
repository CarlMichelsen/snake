import type {FC} from "react";
import type {ChatMessage} from "../model/snake/chatMessage";

type ChatMessageCardProps = {
    isCurrentUser: boolean;
    chatMessage: ChatMessage;
}

const ChatMessageCard: FC<ChatMessageCardProps> = ({ isCurrentUser, chatMessage }) => {
    
    return (
        <div>
            {!isCurrentUser && <p className="text-xs">{chatMessage.sender.name}</p>}
            <div className={`p-2 ${isCurrentUser ? "lg:ml-42 ml-6" : "lg:mr-42 mr-6"} rounded-md dark:bg-neutral-800 bg-neutral-200`}>
                <p className="text-md">{chatMessage.content}</p>
            </div>
        </div>
    );
}

export default ChatMessageCard;