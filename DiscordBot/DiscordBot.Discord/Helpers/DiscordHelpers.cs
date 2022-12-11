namespace DiscordBot.Discord.Helpers;

public static partial class DiscordHelpers
{
    public static string GetUserAndDiscriminator(this IUser sgu) => $"{sgu.Username}#{sgu.Discriminator}";

    public static bool IsAdmin(this SocketCommandContext ctx)
    {
        var roles = ctx.Guild.Roles.Where(x => x.Members.Select(x => x.Id).Contains(ctx.User.Id));
        return roles.Any(x => x.Permissions.Administrator) || ctx.User.Id == ctx.Guild.OwnerId;
    }

    public static ulong OwnerId = 0;

    public static bool IsOwner(this IUser sgu) => sgu.Id == 301764235887902727;
}
