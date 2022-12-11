namespace DiscordBot.Discord.Services.CommandHandler;

public partial class CommandHandler : ICommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly ILogger<CommandHandler> _logger;
    private readonly IConfiguration _configuration;

    public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client, IConfiguration config, ILogger<CommandHandler> logger)
    {
        _commands = commands;
        _services = services;
        _client = client;
        _logger = logger;
        _configuration = config;
    }

    public async Task InitializeAsync()
    {
         await _commands.AddModulesAsync(GetType().Assembly, _services);

        _commands.CommandExecuted += CommandExecutedAsync;
        _commands.Log += LogAsync;

        _client.MessageReceived += Client_HandleCommandAsync;
        _client.Ready += Client_Ready;
        _client.Log += LogAsync;
        _client.Disconnected += Client_Disconnected;
    }

    public async Task LogAsync(LogMessage logMessage)
    {
        if (logMessage.Exception is CommandException cmdException)
        {
             await cmdException.Context.Channel.SendMessageAsync("Something went catastrophically wrong!");
            _logger.LogError(logMessage.Exception, "{user} failed to execute '{name}' in {channel}.", cmdException.Context.User, cmdException.Command.Name, cmdException.Context.Channel);
        }
    }

    public async Task Client_Ready()
    {
        await _client.SetGameAsync("");
        _logger.LogInformation("Bot is online!");
    }

    public async Task Client_HandleCommandAsync(SocketMessage arg)
    {
        if (arg is not SocketUserMessage msg)
            return;

        var context = new SocketCommandContext(_client, msg);

        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot)
            return;

        var prefix = _configuration["Discord:Settings:CommandPrefix"];
        var pos = 0;

        if (msg.HasStringPrefix(prefix, ref pos, StringComparison.OrdinalIgnoreCase) || msg.HasMentionPrefix(_client.CurrentUser, ref pos))
        {
             await _commands.ExecuteAsync(context, pos, _services);
        }
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

    public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        // error happened
        if (!command.IsSpecified)
        {
             await context.Channel.SendMessageAsync($"I don't know this command: {context.Message}");
            return;
        }

        if (result is DiscordBotResult runTimeResult)
        {
            if (command.Value.Module.Name == "Admin")
            {
                _ = DeleteMessage(context.Message, runTimeResult.AnswerSent!);
                return;
            }

            if (result.IsSuccess)
            {
                if (runTimeResult.Reason is not null)
                {
                    var m = await context.Channel.SendMessageAsync(runTimeResult.Reason);

                    if (command.Value.Module.Name == "Admin")
                         _ = DeleteMessage(context.Message, m);
                }
                return;
            }

             await context.Channel.SendMessageAsync($"{runTimeResult.Error}: {runTimeResult.Reason}");

            var member = context.User.GetUserAndDiscriminator();
            var moduleName = command.Value.Module.Name;
            var commandName = command.Value.Name;

            _logger.LogError($"{member} tried to use {commandName} (module: {moduleName}) this resultet in a {runTimeResult?.Error?.ToString()}");
        }
    }

    private static async Task DeleteMessage(IMessage userMsg, IMessage answer)
    {
        await Task.Delay(TimeSpan.FromMinutes(2));

        if (userMsg is not null)
            await userMsg.DeleteAsync(new RequestOptions { AuditLogReason = "Autoremoved" });

        if (answer is not null)
            await answer.DeleteAsync(new RequestOptions { AuditLogReason = "Autoremoved" });
    }
}
