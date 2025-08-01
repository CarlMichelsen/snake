namespace Domain.Snake;

public record ChatMessage(
    Guid Id,
    User Sender,
    string Content,
    DateTimeOffset TimeStamp);