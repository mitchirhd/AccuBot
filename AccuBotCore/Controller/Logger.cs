using Discord;

namespace AccuBotCore.Controller
{
    public class ConsoleLogger
    {
        public static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
    }
}
