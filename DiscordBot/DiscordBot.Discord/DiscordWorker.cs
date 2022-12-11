namespace DiscordBot.Discord;

public class DiscordWorker : BackgroundService
{
    private readonly ILogger<DiscordWorker> _logger;
    private readonly IConfiguration _config;
    private readonly IHostEnvironment _hostEnvironment;

    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ICommandHandler _commandHandler;

    public DiscordWorker(IConfiguration config, IHostEnvironment hostEnvironment, ILogger<DiscordWorker> logger,
        DiscordSocketClient discordSocketClient, ICommandHandler commandHandler)
            => (_config, _hostEnvironment, _logger, _discordSocketClient,_commandHandler)
                = (config, hostEnvironment, logger, discordSocketClient, commandHandler);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTime.UtcNow);
        await InitAsync();
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StartAsync(CancellationToken cancellationToken) 
        => await base.StartAsync(cancellationToken);

    public override async Task StopAsync(CancellationToken cancellationToken) 
        => await Task.Run(Dispose, cancellationToken);

    public async Task InitAsync()
    {
        var token = _hostEnvironment.IsDevelopment() ? _config["Discord:DevToken"] : _config["Discord:Token"];

        _discordSocketClient.Log += LogAsync;
        _discordSocketClient.Disconnected += Client_Disconnected;

        await _discordSocketClient.LoginAsync(TokenType.Bot, token);
        await _discordSocketClient.StartAsync();

        await _commandHandler.InitializeAsync();
    }

    public Task LogAsync(LogMessage m)
    {
        _logger.Log(m.Severity.GetLogLevel(), "{message}", m.Message);
        return Task.CompletedTask;
    }

    public async Task Client_Disconnected(Exception e)
    {
        _logger.LogError(e, "Client disconnected");

        await Task.Run(() =>
        {
            var DiscordBot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
            Process.Start(DiscordBot);
            Environment.Exit(0);
        });
    }
}
