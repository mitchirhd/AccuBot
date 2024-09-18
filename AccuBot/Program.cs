using AccuBot;
using AccuBotCore;
using LightInject;

public class Program
{
    public static async Task Main()
        => await new MainContainer().Get().GetInstance<ClientRunner>().Run();
}