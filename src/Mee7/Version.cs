namespace Mee7;

public static class Version
{
    private static int Major => 0;
    private static int Minor => 1;
    private static int Patch => 0;

    public static string ShortVersion => $"{Major}.{Minor}";
    public static string MediumVersion => $"{Major}.{Minor}.{Patch}";
    public static string DiscordVersion => Discord.DiscordConfig.Version;
        
    public static System.Version AsSystemVersion { get; } = new(Major, Minor, Patch);
}