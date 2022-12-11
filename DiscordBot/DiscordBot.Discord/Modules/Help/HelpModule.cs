namespace DiscordBot.Discord.Modules.Help;

[Name("help")]
public class HelpModule : DiscordBotModule, IHelpModule
{
    private readonly CommandService _commandService;
    private readonly IConfiguration _configuration;

    public HelpModule(CommandService commandService, ICommandHandler commandHandler, IConfiguration configuration) : base(commandHandler) 
        => (_commandService, _configuration) = (commandService, configuration);

    [Command("help")]
    public async Task<RuntimeResult> HelpAsync()
    {
        var prefix = _configuration["Discord:Settings:CommandPrefix"];

        var builder = new EmbedBuilder()
        {
            Color = new Color(114, 137, 218),
            Description = "These are the commands you can use:",
            Footer = new EmbedFooterBuilder()
            {
                Text = "To get more information for each command add the command name behind the help command!"
            }
        };

        foreach (var module in _commandService.Modules)
        {
            var description = new StringBuilder();

            foreach (var cmd in module.Commands.Distinct())
            {
                var result = await cmd.CheckPreconditionsAsync(Context);
                if (!result.IsSuccess)
                    continue;

                var args = string.Join(" ", cmd.Parameters?.Select(x => $"[{x.Name}]").ToArray() ?? Array.Empty<string>());

                if (string.Equals(cmd.Name, module.Group, StringComparison.InvariantCultureIgnoreCase))
                    description.AppendLine($"{prefix}{module.Group} {args}");
                else if (string.IsNullOrWhiteSpace(module.Group))
                    description.AppendLine($"{prefix}{cmd.Name} {args}");
                else
                    description.AppendLine($"{prefix}{module.Group} {cmd.Name} {args}");
            }

            if (!string.IsNullOrWhiteSpace(description.ToString()))
                builder.AddField(x =>
                {
                    x.Name = module.Name;
                    x.Value = description.ToString();
                    x.IsInline = false;
                });
        }

        await ReplyAsync(string.Empty, false, builder.Build());
        return FromSuccess();
    }

    [Command("help")]
    public async Task<RuntimeResult> HelpAsync([Remainder] string command)
    {
        var result = _commandService.Search(Context, command);

        if (!result.IsSuccess)
            return FromErrorObjectNotFound("command", command);

        var builder = new EmbedBuilder()
        {
            Color = new Color(114, 137, 218),
            Description = $"Here are some commands like **{command}**"
        };

        foreach (var match in result.Commands)
        {
            var cmd = match.Command;

            builder.AddField(x =>
            {
                x.Name = string.Join(", ", cmd.Aliases);
                x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                          $"Summary: {cmd.Summary}";
                x.IsInline = false;
            });
        }

        await ReplyAsync("", false, builder.Build());
        return FromSuccess();
    }
}
