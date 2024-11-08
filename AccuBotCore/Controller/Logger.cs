using Discord;

namespace AccuBotCore.Controller
{
    public static class Logger
    {
        private static readonly string _logFileName = "AccuBot.log";
        private static readonly string _logFilePath = Path.Combine(AppContext.BaseDirectory, _logFileName);

        public static Task LogAsync(LogMessage msg)
        {
            Log(msg);
            return Task.CompletedTask;
        }

        public static void Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            LogInFile(msg);
        }

        private static void LogInFile(LogMessage msg)
        {
            if (!File.Exists(_logFilePath))
                File.Create(_logFilePath).Close();

            using var file = new StreamWriter(_logFileName, true);
            file.WriteLine($"{DateTime.Now.Date.ToString("dd.MM.yyyy")} {msg.ToString()}");
        }
    }
}
