namespace DiscordBot.Discord.Contracts.Modules;

public interface IAdminModule
{
    Task<RuntimeResult> Restart();
}
