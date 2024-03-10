using Discord.Interactions;

namespace Mee7.Modules;

public class TestModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("ping", "pong!")]
    public async Task PingAsync()
    {
        // Context.Interaction.
        await Context.Interaction.RespondAsync("Pong!");
    }
}