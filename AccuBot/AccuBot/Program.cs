using AccuBot.Controller;
using Discord;
using Discord.WebSocket;

public class Program
{
    private static DiscordSocketClient _client;

    public static async Task Main()
    {
        _client = new DiscordSocketClient();

        _client.Log += ConsoleLogger.Log;
        var token = File.ReadAllText("token.txt");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}