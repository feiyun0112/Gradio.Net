namespace Demo.AspNetCore.Launcher;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddLauncher();
        var app = builder.Build();
        app.UseLauncher();
        await app.RunAsync();
    }
}
