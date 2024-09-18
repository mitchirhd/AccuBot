using AccuBotCore;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LightInject;

namespace AccuBot
{
    public class MainContainer
    {
        private ServiceContainer _container;
        
        public MainContainer() 
        { 
            _container = new ServiceContainer();
            Load();
        }

        public ServiceContainer Get() => _container;

        private void Load()
        {
            _container.Register<ClientRunner>();
            _container.Register<DiscordSocketClient>();
            _container.RegisterInstance(new DiscordSocketConfig() { GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent });
            _container.Register<CommandService>();
        }
    }
}
