using AccuBotCore.Enum;
using AccuBotCore.Factory;
using AccuBotCore.Jobs.Managers;
using CliWrap;
using Discord;
using Discord.Commands;

namespace AccuBotCore.Modules
{
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        private SongPlayerJobManager _songPlayerManager;
        private QueueStringFactory _queueStringFactory;
        private HelpStringFactory _helpStringFactory;

        public MainModule(SongPlayerJobManager songPlayerJobManager, QueueStringFactory queueStringFactory, HelpStringFactory helpStringFactory)
        {
            _songPlayerManager = songPlayerJobManager;
            _queueStringFactory = queueStringFactory;
            _helpStringFactory = helpStringFactory;
        }

        [Command("play", Aliases = ["p"], RunMode = RunMode.Async)]
        public async Task OnPlay([Remainder] string url)
        {
            var audioChannel = (Context.User as IGuildUser).VoiceChannel;
            await _songPlayerManager.AddSong(audioChannel, url, Context.Channel);
        }

        [Command("playnext", Aliases = ["pn", "pnext"], RunMode = RunMode.Async)]
        public async Task OnPlayNext([Remainder] string url)
        {
            var audioChannel = (Context.User as IGuildUser).VoiceChannel;
            await _songPlayerManager.AddSong(audioChannel, url, Context.Channel, true);
        }

        [Command("skip", RunMode = RunMode.Async)]
        public async Task OnSkip()
        {
            var audioChannel = (Context.User as IGuildUser).VoiceChannel;

            if (!_songPlayerManager.HasJob(audioChannel))
            {
                await ReplyAsync("Must be in same voice channel");
                return;
            }

            var job = await _songPlayerManager.GetJob(audioChannel);
            job?.SkipSong();
        }

        [Command("clear", RunMode = RunMode.Async)]
        public async Task OnClear([Remainder] string param = "")
        {
            var audioChannel = (Context.User as IGuildUser).VoiceChannel;

            if (!_songPlayerManager.HasJob(audioChannel))
            {
                await ReplyAsync("Must be in same voice channel");
                return;
            }

            var clearMode = ParseQueueParamMode(param);
            var job = await _songPlayerManager.GetJob(audioChannel);
            job?.Clear(clearMode);
            await ReplyAsync("Cleared");
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task OnLeave()
        {
            var audioChannel = (Context.User as IGuildUser).VoiceChannel;
            await _songPlayerManager.Leave(audioChannel, Context.Channel);
        }

        [Command("shuffle", RunMode = RunMode.Async)]
        public async Task OnShuffle([Remainder] string param = "")
        {
            var audioChannel = (Context.User as IGuildUser).VoiceChannel;

            if (!_songPlayerManager.HasJob(audioChannel))
            {
                await ReplyAsync("Must be in same voice channel");
                return;
            }

            var shuffleMode = ParseQueueParamMode(param);
            var job = await _songPlayerManager.GetJob(audioChannel);
            job?.Shuffle(shuffleMode);

            await ReplyAsync("Queue shuffled");
        }

        [Command("queue", Aliases = ["q"], RunMode = RunMode.Async)]
        public async Task OnQueue()
        {
            var audioChannel = (Context.User as IGuildUser).VoiceChannel;
            var job = await _songPlayerManager.GetJob(audioChannel);
            var queueString = _queueStringFactory.Create(job?.GetQueue());
            await ReplyAsync(queueString);
        }

        [Command("nowplaying", Aliases = ["np"], RunMode = RunMode.Async)]
        public async Task OnNowPlaying()
        {
            var audioChannel = (Context.User as IGuildUser).VoiceChannel;
            var job = await _songPlayerManager.GetJob(audioChannel);
            var queueString = _queueStringFactory.CreateNowPlayíng(job?.GetQueue().Item1);
            await ReplyAsync(queueString);
        }

        [Command("help", RunMode = RunMode.Async)]
        public async Task OnHelp()
        {
            await ReplyAsync(_helpStringFactory.Create());
        }

        private QueueParamModes ParseQueueParamMode(string param)
        {
            QueueParamModes mode = QueueParamModes.All;
            if (string.IsNullOrEmpty(param) || string.Equals(param, "all", StringComparison.InvariantCultureIgnoreCase))
                mode = QueueParamModes.All;
            else if (string.Equals(param, "prio", StringComparison.InvariantCultureIgnoreCase))
                mode = QueueParamModes.PrioQueue;
            else if (string.Equals(param, "queue", StringComparison.InvariantCultureIgnoreCase))
                mode = QueueParamModes.Queue;

            return mode;
        }
    }
}
