using System.ComponentModel.DataAnnotations;
using Presentation.Configuration.Options;

namespace Application.Configuration.Options;

public class SnakeOptions : IConfigurationOptions
{
    public static string SectionName { get; } = "Snake";
    
    [Required]
    public required DiscordWebhookOptions DiscordWebhook { get; init; }
}