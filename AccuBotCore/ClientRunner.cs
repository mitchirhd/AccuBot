using AccuBotCMD;
using AccuBotCore.Controller;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace AccuBotCore
{
    public class ClientRunner
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        public ClientRunner(DiscordSocketClient client, CommandService cmdService) 
        {
            _client = client;
            _commandService = cmdService;
        }

        public async Task Run()
        {
            _client.Log += ConsoleLogger.Log;
            _commandService.Log += ConsoleLogger.Log;
            var token = File.ReadAllText("token.txt");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            CommandHandler handler = new CommandHandler(_client, _commandService);
            await handler.InstallCommandsAsync();

            await Task.Delay(-1);
        }
    }
}
