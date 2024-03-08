using Serilog;
using System.Text.Json;

namespace Mee7;

public class AlphaConfig
{
    private const string c_configPath = "config.json";
    public static async Task<AlphaConfig?> LoadAsync()
    {
        if (!File.Exists(c_configPath))
        {
            Log.Error("Unable to locate {c_configPath}, please ensure it's in the root of the project when starting the app", c_configPath);
            return null;
        }
        
        await using var fs = File.OpenRead(c_configPath);
        var config = await JsonSerializer.DeserializeAsync<AlphaConfig>(fs, JsonSerializerOptions.Default);
        if (config is not null)
        {
            return config;
        }        
        
        Log.Error("Unable to load config.json, please ensure it exists and is populated with valid information");
        return null;
    }

    public string Token { get; init; } = string.Empty;
}