using Serilog;
using Serilog.Events;

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Addons.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mee7;

using Services;

public static class BotHost
{
    public static async Task<int> RunAsync(string[] args)
    {
        var path = Path.Combine("logs", "log.txt");
        if (!Directory.Exists("logs"))
        {
            Directory.CreateDirectory("logs");
        }
        
        Log.Information("Starting Mee7 v{MediumVersion} with Discord.NET v{DiscordVersion}", Version.MediumVersion, Version.DiscordVersion);
            
        var alphaConfig = await AlphaConfig.LoadAsync();
        if (alphaConfig is null)
        {
            Log.Error("Config file was null, unable to start bot");
            return 1;
        }
        
        var host = CreateHostBuilder(args, path, alphaConfig).Build();

        // Try catch so we can monitor when something goes wonky, will probably end up using something like Docker to host anyways
        try
        {
            await host.RunAsync();
            return 0;
        }
        catch (Exception e) // This shit happens when Ctrl+C the app sometimes...
        {
            // NOTE:
            // - This exception likely occurred due to a lack of permissions (intents) with the bot token you're using.
            //   Check the Discord Developer Portal and ensure your intents match our configs.
            if (e is WebSocketClosedException webSocketClosedException)
            {
                Log.Error("{0}", webSocketClosedException);
            }

            return 1;
        }
        finally
        {
            host.Dispose();
            await Log.CloseAndFlushAsync();
        }
    }

    // Info specifically on Dependency Injection - https://discordnet.dev/guides/dependency_injection/basics.html
    private static IHostBuilder CreateHostBuilder(string[] args, string logPath, AlphaConfig alphaConfig)
    {
        return Host.CreateDefaultBuilder(args)
            // NOTE:
            // - I'm gonna create a custom sink for the logger at a later date, don't worry about log formatting.
            //   Any time you want to output something to the console you can just call `Log.Error/Information/Etc("My Message")`
            //   Everything is already setup
            .UseSerilog(new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .WriteTo.Async(x =>
                {
                    x.Console();
                    x.RollingFile(logPath, flushToDiskInterval: TimeSpan.FromHours(24));
                })
                .CreateLogger()
            )
            .ConfigureServices((_, services) =>
            {
                // Sharding the bot allows it to run on multiple servers without issues 
                services.AddDiscordShardedHost((config, _) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200,
                        GatewayIntents = GatewayIntents.All,
                        LogGatewayIntentWarnings = false,
                    };
                    
                    config.Token = alphaConfig.Token;
                });
                
                services.AddInteractionService((config, _) =>
                {
                    config.DefaultRunMode = Discord.Interactions.RunMode.Async;
                    config.UseCompiledLambda = true;
                    config.EnableAutocompleteHandlers = true;
                });
                
                services.AddSingleton(alphaConfig);
                services.AddHostedService<CommandHandler>();
            })
            .UseConsoleLifetime();
    }
}