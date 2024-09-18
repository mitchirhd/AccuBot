using CliWrap;
using Discord;
using Discord.Audio;
using Discord.Commands;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace AccuBotCMD.Modules
{
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task EchoAsync([Remainder] string echo)
        {
            await ReplyAsync(echo);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task OnPlay()
        {
            var channel = (Context.User as IGuildUser).VoiceChannel;
            if (channel == null)
            {
                await ReplyAsync("Must be in voice channel");
                return;
            }
            var audioClient = await channel.ConnectAsync();
            await PlaySong(audioClient, "https://www.youtube.com/watch?v=9bZkp7q19f0");
        }

        private async Task PlaySong(IAudioClient AudioClient, string httplink)
        {
            var youtube = new YoutubeClient();
            StreamManifest streamManifest = await youtube.Videos.Streams.GetManifestAsync(httplink);
            IStreamInfo streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            Stream InputStream = await youtube.Videos.Streams.GetAsync(streamInfo);

            MemoryStream Memory = new MemoryStream();
            await Cli.Wrap("ffmpeg").WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                                    .WithStandardInputPipe(PipeSource.FromStream(InputStream))
                                    .WithStandardOutputPipe(PipeTarget.ToStream(Memory))
                                    .ExecuteAsync();

            using (AudioOutStream OutputStream = AudioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await OutputStream.WriteAsync(Memory.ToArray(), 0, (int)Memory.Length); }
                finally { await OutputStream.FlushAsync(); }
            }
        }
    }
}
