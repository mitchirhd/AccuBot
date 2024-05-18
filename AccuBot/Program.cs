using AccuBot.Controller;
using Discord;
using Discord.WebSocket;

public class Program
{
    public static async Task Main()
    {
        var client = new DiscordSocketClient();

        client.Log += ConsoleLogger.Log;
        var token = File.ReadAllText("token.txt");

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}