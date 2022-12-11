namespace DiscordBot.Discord;

public static class DiscordHosts
{
    public static IHostBuilder CreateDiscordSocketHost(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .UseSystemd()
           .ConfigureAppConfiguration((hostContext, configBuilder) =>
           {
               try
               {
                    configBuilder.AddEnvironmentVariables("DiscordBot_")
                                 .AddUserSecrets<DiscordWorker>();
               }
               catch
               {
                   // ignore
               }
           })
           .UseSerilog((h, l) => l.ReadFrom.Configuration(h.Configuration))
           .ConfigureServices((hostContext, services) =>
           {
               var config = hostContext.Configuration;

                services
                   .AddHostedService<DiscordWorker>()
                   .AddSingleton(o => new DiscordSocketClient(new DiscordSocketConfig
                   {
                       LogLevel = Enum.Parse<LogSeverity>(config["Discord:Settings:LogLevel"]!, true),
                       MessageCacheSize = int.Parse(config["Discord:Settings:MessageCacheSize"]!),
                       GatewayIntents = GatewayIntents.All, 
                   }))
                   .AddSingleton(s => new CommandService(new CommandServiceConfig
                   {
                       LogLevel = Enum.Parse<LogSeverity>(config["Discord:Settings:LogLevel"]!, true),
                       CaseSensitiveCommands = bool.Parse(config["Discord:Settings:CaseSensitiveCommands"]!),
                       DefaultRunMode = Enum.Parse<RunMode>(config["Discord:Settings:DefaultRunMode"]!, true),
                       SeparatorChar = config["Discord:Settings:SeparatorChar"]!.FirstOrDefault(),
                   }))
                   .AddSingleton<ICommandHandler, CommandHandler>()
                   .AddScoped<IAdminService, AdminService>()
                   .AddHttpClient()
                   .BuildServiceProvider();
           });

    public static IHostBuilder CreateDiscordWebhookHost(string[] args, ulong webhookId, string webhookToken,
        Func<DiscordWebhookClient, Task> action, CancellationToken token)
        => Host.CreateDefaultBuilder(args)
               .ConfigureServices(async (c, x) =>
               {
                   var client = new DiscordWebhookClient(webhookId, webhookToken);

                   while(!token.IsCancellationRequested)
                   {
                       await action(client);
                   }
               });
}
