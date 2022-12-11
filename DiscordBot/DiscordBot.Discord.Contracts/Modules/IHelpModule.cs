namespace DiscordBot.Discord.Contracts.Modules;

public interface IHelpModule
{
    Task<RuntimeResult> HelpAsync();
    Task<RuntimeResult> HelpAsync([Remainder] string command);
}
