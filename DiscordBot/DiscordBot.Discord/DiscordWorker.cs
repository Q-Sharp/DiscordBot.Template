namespace DiscordBot.Discord;

public class DiscordWorker : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<DiscordWorker> _logger;
    private readonly IConfiguration _config;
    private readonly IHostEnvironment _env;

    public DiscordWorker(IServiceProvider sp, ILogger<DiscordWorker> logger, IConfiguration config, IHostEnvironment env)
    {
        _sp = sp;
        _logger = logger;
        _config = config;
        _env = env;
    }

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
        var client = _sp.GetRequiredService<DiscordSocketClient>();
        var commandService = _sp.GetRequiredService<CommandService>();
        var ch = _sp.GetRequiredService<ICommandHandler>();
        var token = _env.IsDevelopment() ? _config["Discord:DevToken"] : _config["Discord:Token"];

        commandService.Log += LogAsync;
        client.Log += LogAsync;

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await ch.InitializeAsync();
    }

    public Task LogAsync(LogMessage m)
    {
        _logger.Log(GetLogLevel(m.Severity), "{message}", m.Message);
        return Task.CompletedTask;
    }

    private LogLevel GetLogLevel(LogSeverity ls)
        => ls switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.None
        };
}
