using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccuBotCMD.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task EchoAsync([Remainder] string echo)
        {
            await ReplyAsync(echo);
        }
    }
}
