using AccuBotCore;
using AccuBotCore.Factory;
using AccuBotCore.Jobs.Managers;
using AccuBotCore.Provider;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

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
            .AddSingleton<SongFactory>()
            .AddSingleton<QueueStringFactory>()
            .AddSingleton<HelpStringFactory>()
            .AddSingleton<IYoutubeInfoProvider, YoutubeExplodeInfoProvider>()
            .AddSingleton<SongPlayerJobFactory>();

        return collection.BuildServiceProvider();
    }
}