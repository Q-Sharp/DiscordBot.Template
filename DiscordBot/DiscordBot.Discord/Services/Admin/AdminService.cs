namespace DiscordBot.Discord.Services.Admin;

public class AdminService : IAdminService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public AdminService(IHostApplicationLifetime hostApplicationLifetime) 
        => _hostApplicationLifetime = hostApplicationLifetime;

    public void Restart() 
        => _hostApplicationLifetime?.StopApplication();
}
