using Discord.Commands;
using Discord.Interactions;

namespace Mee7.Modules;

public class TestModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("ping", "pong!")]
    public async Task PingAsync()
    {
        await Context.Interaction.RespondAsync("Pong!");
    }

    [SlashCommand("mimic", "mimics back an input")]
    public async Task MimicAsync([Remainder] string text)
    {
        await Context.Interaction.RespondAsync(text);
    }
}