using AccuBotCore.Factory;
using Discord;
using Discord.Audio;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace AccuBotCore.Jobs.Managers
{
    public class SongPlayerJobManager
    {
        private Dictionary<IAudioChannel, (IAudioClient, SongPlayerJob, IMessageChannel)> _jobMapping = new Dictionary<IAudioChannel, (IAudioClient, SongPlayerJob, IMessageChannel)>();
        private YoutubeClient _ytClient;
        private SongFactory _songFactory;

        public SongPlayerJobManager(YoutubeClient youtubeClient, SongFactory songFactory) 
        { 
            _ytClient = youtubeClient;
            _songFactory = songFactory;
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
                var videos = await _ytClient.Playlists.GetVideosAsync(url);
                foreach (var video in videos)
                {
                    job?.AddSong(_songFactory.Create(video), prio);
                }
                await textChannel.SendMessageAsync($"Added {videos.Count} entries to queue");
            }
            else
            {
                var video = await _ytClient.Videos.GetAsync(url);
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
                var value = (audioClient, new SongPlayerJob(audioClient, textChannel, _ytClient), textChannel);
                _jobMapping[audioChannel] = value;
                value.Item2.PlayingSongEvent += OnPlayingSongEvent;
            }
            return _jobMapping[audioChannel].Item2;
        }

        public bool HasJob(IAudioChannel channel)
        {
            return _jobMapping.Keys.Contains(channel);
        }

        private async void OnPlayingSongEvent(SongPlayerJob job, Video videoInfo)
        {
            var textChannel = _jobMapping.FirstOrDefault(kvp => kvp.Value.Item2 == job).Value.Item3;
            await textChannel.SendMessageAsync($"Playing {Format.Bold(videoInfo.Title)}\n{videoInfo.Url}");
        }
    }
}
