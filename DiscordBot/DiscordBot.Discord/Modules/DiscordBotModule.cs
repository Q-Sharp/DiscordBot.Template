namespace DiscordBot.Discord.Modules;

public abstract class DiscordBotModule : ModuleBase<SocketCommandContext>
{
    protected ICommandHandler _commandHandler;

    public DiscordBotModule(ICommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public static DiscordBotResult FromSuccess(string? successMessage = null, IMessage? answer = null)
        => DiscordBotResult.Create(null, successMessage, answer);
    public static DiscordBotResult FromError(CommandError error, string? reason, IMessage? answer = null)
        => DiscordBotResult.Create(error, reason, answer);
    public static DiscordBotResult FromErrorObjectNotFound(string? objectname, string? searchstring, IMessage? answer = null)
        => DiscordBotResult.Create(CommandError.ObjectNotFound, $"{objectname}: {searchstring}", answer);
    public static DiscordBotResult FromErrorUnsuccessful(string? error, IMessage? answer = null)
        => DiscordBotResult.Create(CommandError.Unsuccessful, error, answer);
    public static DiscordBotResult FromIgnore()
        => DiscordBotResult.Create(null, null, null);
}
