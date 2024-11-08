using AccuBotCore.Factory;
using AccuBotCore.Provider;
using Discord;
using Discord.Audio;

namespace AccuBotCore.Jobs.Managers
{
    public class SongPlayerJobManager
    {
        private Dictionary<IAudioChannel, (IAudioClient, SongPlayerJob)> _jobMapping = new Dictionary<IAudioChannel, (IAudioClient, SongPlayerJob)>();
        private IYoutubeInfoProvider _youtubeInfoProvider;
        private SongFactory _songFactory;
        private SongPlayerJobFactory _songPlayerJobFactory;

        public SongPlayerJobManager(IYoutubeInfoProvider youtubeInfoProvider, SongFactory songFactory, SongPlayerJobFactory songPlayerJobFactory)
        {
            _songFactory = songFactory;
            _songPlayerJobFactory = songPlayerJobFactory;
            _youtubeInfoProvider = youtubeInfoProvider;
        }

        public async Task AddSong(IAudioChannel audioChannel, string url, IMessageChannel textChannel, bool prio = false)
        {
            if(audioChannel == null)
            {
                await textChannel.SendMessageAsync("Must be in voice channel");
                return;
            }

            SongPlayerJob? job;
            if (HasJob(audioChannel) && !_jobMapping.ContainsKey(audioChannel))
            {
                await textChannel.SendMessageAsync("Must be in same voice channel");
                return;
            }

            job = await GetJob(audioChannel, true, textChannel);
            if (url.Contains("playlist", StringComparison.InvariantCultureIgnoreCase))
            {
                var videos = await _youtubeInfoProvider.GetPlaylistVideos(url);
                foreach (var video in videos)
                {
                    job?.AddSong(_songFactory.Create(video), prio);
                }
                await textChannel.SendMessageAsync($"Added {videos.Count} entries to queue");
            }
            else
            {
                var video = await _youtubeInfoProvider.GetVideoInfo(url);
                job?.AddSong(_songFactory.Create(video), prio);
                await textChannel.SendMessageAsync($"Added entry to queue");
            }
        }

        public async Task Leave(IAudioChannel audioChannel, IMessageChannel textChannel)
        {
            if (!HasJob(audioChannel))
            {
                await textChannel.SendMessageAsync("Must be in same voice channel");
                return;
            }

            var job = await GetJob(audioChannel);
            job?.Leave();
            _jobMapping.Remove(audioChannel);
        }

        public async Task<SongPlayerJob?> GetJob(IAudioChannel audioChannel, bool create = false, IMessageChannel? textChannel = null)
        {
            if (!_jobMapping.ContainsKey(audioChannel))
            {
                if (!create)
                    return null;
                if (textChannel == null)
                    throw new Exception("When creating a new entry for _jobMapping, a textChannel is needed");

                var audioClient = await audioChannel.ConnectAsync();
                var job = _songPlayerJobFactory.CreateSongPlayerJob(audioClient, textChannel);
                var value = (audioClient, job);
                _jobMapping[audioChannel] = value;
            }
            return _jobMapping[audioChannel].Item2;
        }

        public bool HasJob(IAudioChannel channel)
        {
            return _jobMapping.Keys.Contains(channel);
        }
    }
}
