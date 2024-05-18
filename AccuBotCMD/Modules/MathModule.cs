using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccuBotCMD.Modules
{
    [Group("math")]
    public class MathModule : ModuleBase<SocketCommandContext>
    {
        [Command("pi")]
        public async Task Pi(int num)
        {
            await Context.Channel.SendMessageAsync(Math.PI.ToString());
        }
    }
}
