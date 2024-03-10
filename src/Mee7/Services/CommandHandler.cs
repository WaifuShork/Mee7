using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Mee7.Services;

public class CommandHandler(IServiceProvider provider, InteractionService interactionService, DiscordShardedClient _, ILogger<DiscordShardedClientService> __) : DiscordShardedClientService(_, __)
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken).ContinueWith(async _ =>
        {
            await RegisterCommandsAsync().ConfigureAwait(false);
            
            Client.InteractionCreated += OnInteractionCreatedAsync;
            interactionService.SlashCommandExecuted += OnSlashCommandExecutedAsync;
        }, stoppingToken).ConfigureAwait(false);
    }

    private async Task RegisterCommandsAsync()
    {
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), provider).ConfigureAwait(false);
        await interactionService.RegisterCommandsGloballyAsync().ConfigureAwait(false);
    }

    private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
    {
        var scope = provider.CreateAsyncScope();
        var ctx = new ShardedInteractionContext(Client, interaction);
        await interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider).ConfigureAwait(false);
    }

    private Task OnSlashCommandExecutedAsync(SlashCommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        // TODO: handle errors and what not for command execution
        return Task.CompletedTask;
    }
    
    public override void Dispose()
    {
        Client.InteractionCreated -= OnInteractionCreatedAsync;
        interactionService.SlashCommandExecuted -= OnSlashCommandExecutedAsync;
        GC.SuppressFinalize(this);
    }
}