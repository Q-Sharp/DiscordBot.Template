namespace DiscordBot.Discord.Filters;

public class RequireBotOwnerAttribute : RequireOwnerAttribute
{
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        => context.User.IsOwner()
            ? Task.FromResult(FromSuccess())
            : base.CheckPermissionsAsync(context, command, services);
}
