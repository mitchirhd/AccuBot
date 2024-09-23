using AccuBotCore.Controller;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AccuBotCore
{
    public class ClientRunner
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        public ClientRunner(DiscordSocketClient client, CommandService cmdService, IServiceProvider serviceProvider) 
        {
            _client = client;
            _commandService = cmdService;
            _serviceProvider = serviceProvider;
        }

        public async Task Run()
        {
            _client.Log += ConsoleLogger.Log;
            _commandService.Log += ConsoleLogger.Log;
            var token = File.ReadAllText("token.txt");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            CommandHandler handler = _serviceProvider.GetRequiredService<CommandHandler>();
            await handler.InstallCommandsAsync();

            await Task.Delay(-1);
        }
    }
}
