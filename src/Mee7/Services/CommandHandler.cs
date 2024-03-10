using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mee7.Services;

// Some info on getting started 
// The library we're using for Dependency Injection and hosted services: https://github.com/Hawxy/Discord.Addons.Hosting
// Discord.NET docs: https://discordnet.dev/guides/introduction/intro.html
// Basic Commands: https://discordnet.dev/guides/text_commands/intro.html
// Slash Commands: https://discordnet.dev/guides/int_basics/application-commands/intro.html
public class CommandHandler(DiscordShardedClient client, ILogger<DiscordShardedClientService> logger, IServiceProvider provider, CommandService commandService, InteractionService interactionService, AlphaConfig alphaConfig) : DiscordShardedClientService(client, logger)
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken).ContinueWith(async _ =>
        {
            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), provider).ConfigureAwait(false);
            await interactionService.RegisterCommandsGloballyAsync().ConfigureAwait(false);
            Client.InteractionCreated += async interaction =>
            {
                var scope = provider.CreateAsyncScope();
                var ctx = new ShardedInteractionContext(Client, interaction);
                await interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider).ConfigureAwait(false);
            };
        }, stoppingToken).ConfigureAwait(false);
    }
}