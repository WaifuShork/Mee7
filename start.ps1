function Start-Bot()
{
    dotnet.exe run --project "src\Mee7\Mee7.csproj" --configuration debug;
}

Start-Bot;