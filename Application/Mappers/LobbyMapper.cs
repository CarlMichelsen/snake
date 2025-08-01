using Domain.Snake;
using Presentation.Hub.Lobby.Dto;

namespace Application.Mappers;

public static class LobbyMapper
{
    public static LobbyDto ToDto(this Lobby lobby)
        => new LobbyDto(
            Id: lobby.Id,
            LobbyLeader: lobby.LobbyLeader.ToDto(),
            Messages: lobby.Messages.Select(m => m.ToDto()).ToList(),
            Users: lobby.Users.ToDictionary(kv => kv.Key, kv => kv.Value.ToDto()));
    
    public static ChatMessageDto ToDto(this ChatMessage chatMessage)
        => new ChatMessageDto(
            Id: chatMessage.Id,
            Sender: chatMessage.Sender.ToDto(),
            Content: chatMessage.Content,
            TimeStamp: chatMessage.TimeStamp);
}