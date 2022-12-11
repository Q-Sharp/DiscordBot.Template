namespace DiscordBot.Discord.Services;

public abstract class DiscordBotService<T>
    where T : class
{
    protected ILogger<T> _logger;
    public DiscordBotService(ILogger<T> logger) => _logger = logger;
}
