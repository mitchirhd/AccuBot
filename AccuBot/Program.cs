using AccuBotCore;
using AccuBotCore.Factory;
using AccuBotCore.Jobs.Managers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using YoutubeExplode;

public class Program
{
    private static IServiceProvider? _serviceProvider;

    public static async Task Main()
    {
        _serviceProvider = CreateProvider();
        await _serviceProvider.GetRequiredService<ClientRunner>().Run();
    }

    private static IServiceProvider CreateProvider()
    {
        var collection = new ServiceCollection()
            .AddSingleton<ClientRunner>()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(new DiscordSocketConfig() { GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent })
            .AddSingleton<CommandHandler>()
            .AddSingleton<SongPlayerJobManager>()
            .AddSingleton<CommandService>()
            .AddSingleton<YoutubeClient>()
            .AddSingleton<SongFactory>()
            .AddSingleton<QueueStringFactory>()
            .AddSingleton<HelpStringFactory>();

        return collection.BuildServiceProvider();
    }
}