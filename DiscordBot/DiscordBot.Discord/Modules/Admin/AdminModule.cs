namespace DiscordBot.Discord.Modules.Admin;

[Name("admin")]
[Group("admin")]
[Alias("admin", "a")]
public partial class AdminModule : DiscordBotModule, IAdminModule
{
    private readonly IAdminService _adminService;

    public AdminModule(IAdminService adminService, ICommandHandler commandHandler)
        : base(commandHandler)
    {
        _adminService = adminService;
    }

    [Command("restart")]
    [Summary("restart the bot")]
    [RequireOwner]
    public async Task<RuntimeResult> Restart()
    {
        await ReplyAsync("Restarting.....");
        _adminService.Restart();
        return FromSuccess();
    }

    [Command("showservers")]
    [Summary("show all servers the bot is member of")]
    [RequireOwner]
    public async Task<RuntimeResult> ShowServers()
    {
        await ReplyAsync(string.Join(Environment.NewLine, Context.Client.Guilds.Select(x => x.Name)));
        return FromSuccess();
    }

}
