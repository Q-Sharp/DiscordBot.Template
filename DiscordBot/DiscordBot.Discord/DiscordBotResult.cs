namespace DiscordBot.Discord;

public class DiscordBotResult : RuntimeResult
{
    private DiscordBotResult(CommandError? error, string? reason = null, IMessage? answer = null) : base(error, reason)
        => AnswerSent = answer;

    public IMessage? AnswerSent { get; set; }

    public static DiscordBotResult Create(CommandError? error, string? reason = null, IMessage? answer = null)
        => new(error, reason, answer);
}
