using AccuBot.Controller;
using AccuBotCMD;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class Program
{
    public static async Task Main()
    {
        var config = new DiscordSocketConfig() { GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent };
        var client = new DiscordSocketClient(config);
        var cmdService = new CommandService();

        client.Log += ConsoleLogger.Log;
        cmdService.Log += ConsoleLogger.Log;
        var token = File.ReadAllText("token.txt");

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        CommandHandler handler = new CommandHandler(client, cmdService);
        await handler.InstallCommandsAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}