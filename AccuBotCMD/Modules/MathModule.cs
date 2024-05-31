using Discord.Commands;

namespace AccuBotCMD.Modules
{
    [Group("math")]
    public class MathModule : ModuleBase<SocketCommandContext>
    {
        [Command("pi")]
        public async Task Pi()
        {
            await Context.Channel.SendMessageAsync(Math.PI.ToString());
        }
    }
}
