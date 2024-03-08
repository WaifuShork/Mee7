namespace Mee7;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        return await BotHost.RunAsync(args);
    }
}