namespace DiscordBot.Discord.Helpers;

public static partial class DiscordHelpers
{
    public static string GetUserAndDiscriminator(this IUser sgu) => $"{sgu.Username}#{sgu.Discriminator}";

    public static bool IsAdmin(this SocketCommandContext ctx)
    {
        var roles = ctx.Guild.Roles.Where(x => x.Members.Select(x => x.Id).Contains(ctx.User.Id));
        return roles.Any(x => x.Permissions.Administrator) || ctx.User.Id == ctx.Guild.OwnerId;
    }

    public static ulong OwnerId { get; set; } = 0;

    public static bool IsOwner(this IUser sgu) => sgu.Id == OwnerId;

    public static LogLevel GetLogLevel(this LogSeverity ls)
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
