using Discord.Commands;

namespace AccuBotCMD.Modules
{
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task EchoAsync([Remainder] string echo)
        {
            await ReplyAsync(echo);
        }
    }
}
