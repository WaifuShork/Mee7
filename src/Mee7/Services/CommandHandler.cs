using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Mee7.Services;

// Some info on getting started 
// The library we're using for Dependency Injection and hosted services: https://github.com/Hawxy/Discord.Addons.Hosting
// Discord.NET docs: https://discordnet.dev/guides/introduction/intro.html
// Basic Commands: https://discordnet.dev/guides/text_commands/intro.html
// Slash Commands: https://discordnet.dev/guides/int_basics/application-commands/intro.html
public class CommandHandler(
    DiscordShardedClient client,
    ILogger<DiscordShardedClientService> logger,
    IServiceProvider provider,
    CommandService commandService,
    AlphaConfig alphaConfig)
    : DiscordShardedClientService(client, logger)
{
    private readonly IServiceProvider m_provider = provider;
    private readonly CommandService m_commandService = commandService;
    private readonly AlphaConfig m_alphaConfig = alphaConfig;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Initializing Command Handler");
        Log.Information($"Look a token: '{m_alphaConfig.Token}'");
        
        Client.MessageCommandExecuted += OnMessageCommandExecutedAsync; // Message based command
        Client.SlashCommandExecuted += OnSlashCommandExecutedAsync;     // Application slash command, these can be more involved
        
        // Adds all classes marked as ModuleBase, remember to use `ModuleBase<ShardedCommandContext>`
        await m_commandService.AddModulesAsync(Assembly.GetEntryAssembly(), m_provider);
    }

    // Normal Command Executed
    private Task OnMessageCommandExecutedAsync(SocketMessageCommand arg)
    {
        Log.Information("Message Command {CommandName}", arg.CommandName);
        return Task.CompletedTask;
    }

    // Slash Command Executed
    private Task OnSlashCommandExecutedAsync(SocketSlashCommand arg)
    {
        Log.Information("Slash Command {CommandName}", arg.CommandName);
        return Task.CompletedTask;
    }
}