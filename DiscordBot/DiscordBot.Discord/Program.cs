Log.Logger  = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

try
{
    using var hb = DiscordHosts.CreateDiscordSocketHost(args)?.Build();
    await hb!.RunAsync();
}
catch(Exception e)
{
    Log.Fatal(e, "FATAL");
}
finally
{
    Log.CloseAndFlush();
}