namespace Presentation.Client.Discord;

public interface IDiscordWebhookMessageClient
{
    Task SendMessageAsync(
        WebhookMessage message,
        CancellationToken cancellationToken = default);
}