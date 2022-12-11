namespace DiscordBot.Discord.Contracts.Services;

public interface ICommandHandler
{
    Task InitializeAsync();
    Task Client_HandleCommandAsync(SocketMessage arg);
    Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result);
}
