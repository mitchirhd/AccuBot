using AccuBotCore.Enum;
using AccuBotCore.Models;
using AccuBotCore.Provider;
using CliWrap;
using Discord;
using Discord.Audio;

namespace AccuBotCore.Jobs
{
    public class SongPlayerJob : JobBase
    {
        private readonly IAudioClient _audioClient;
        private readonly IMessageChannel _msgChannel;
        private IYoutubeInfoProvider _ytProvider;
        private List<Song> _songs = [];
        private List<Song> _songsPrio = [];
        private Task? _currentTask;
        private bool _stopCurrent;
        private bool _isPlaying;
        private Song? _nowPlaying = null;

        public SongPlayerJob(IAudioClient audioClient, IMessageChannel msgChannel, IYoutubeInfoProvider provider) 
        {
            _audioClient = audioClient;
            _msgChannel = msgChannel;
            _ytProvider = provider;

            _ytProvider.ErrorEvent += OnErrorEvent;
        }

        public override void Stop()
        {
            base.Stop();
            _stopCurrent = true;
        }

        public override Task OnExecute()
        {
            if (_songs.Count + _songsPrio.Count == 0)
            {
                Stop();
                return Task.CompletedTask;
            }
            if (_isPlaying || (_currentTask != null && !_currentTask.IsCompleted))
                return Task.CompletedTask;

            bool prio;
            if (_songsPrio.Count > 0)
                prio = true;
            else
                prio = false;

            return StartNewSong(_audioClient, prio);
        }

        private async Task StartNewSong(IAudioClient AudioClient, bool prio)
        {
            if (_currentTask != null && !_currentTask.IsCompleted)
                _currentTask.Wait();

            _nowPlaying = prio ? _songsPrio.First() : _songs.First();
            string url = _nowPlaying.URL;
            Stream inputStream = await _ytProvider.GetAudioStreamAsync(url);
            if(inputStream == Stream.Null)
            {
                if(prio)
                    _songsPrio.RemoveAt(0);
                else
                    _songs.RemoveAt(0);
                return;
            }

            if (prio)
            {
                if (_songsPrio.Count > 0)
                    _songsPrio.RemoveAt(0);
            }
            else if (_songs.Count > 0)
                _songs.RemoveAt(0);

            _isPlaying = true;
            _stopCurrent = false;
            _currentTask = Task.Run(() => PlaySong(inputStream, url));
            await _currentTask;
            _isPlaying = false;
        }

        public async Task PlaySong(Stream inputStream, string url)
        {
            MemoryStream memory = new MemoryStream();
            await Cli.Wrap("ffmpeg").WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                                    .WithStandardInputPipe(PipeSource.FromStream(inputStream))
                                    .WithStandardOutputPipe(PipeTarget.ToStream(memory))
                                    .ExecuteAsync();

            var video = await _ytProvider.GetVideoInfo(url);
            await _msgChannel.SendMessageAsync($"Playing {Format.Bold(video.Title)}\n{video.Url}");
            memory.Seek(0, SeekOrigin.Begin);
            using (AudioOutStream outputStream = _audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                int readBytes = 0;
                byte[] buffer = new byte[4096];
                while (!_stopCurrent && (readBytes = memory.Read(buffer, 0, buffer.Length)) > 0)
                    outputStream.Write(buffer, 0, readBytes);
            }
        }

        public void AddSong(Song song, bool prio = false)
        {
            if (prio)
                _songsPrio.Add(song);
            else
                _songs.Add(song);

            if (IsStopped)
                Start();
        }

        public async void SkipSong()
        {
            if (_currentTask == null || _currentTask.IsCompleted)
                return;

            _stopCurrent = true;
            await _currentTask;
            _stopCurrent = false;
        }

        public void Clear(QueueParamModes mode)
        {
            switch(mode)
            {
                case QueueParamModes.All:
                    _songs.Clear();
                    _songsPrio.Clear();
                    return;
                case QueueParamModes.PrioQueue:
                    _songsPrio.Clear();
                    return;
                case QueueParamModes.Queue:
                    _songs.Clear();
                    return;
                default:
                    return;
            }
        }

        public async void Leave()
        {
            if (_currentTask != null && !_currentTask.IsCompleted)
            {
                Clear(QueueParamModes.All);
                _stopCurrent = true;
                await _currentTask;
            }
            _audioClient.Dispose();
        }

        public void Shuffle(QueueParamModes shuffleMode)
        {
            switch(shuffleMode)
            {
                case QueueParamModes.All:
                    _songsPrio.Shuffle();
                    _songs.Shuffle();
                    return;
                case QueueParamModes.PrioQueue:
                    _songsPrio.Shuffle();
                    return;
                case QueueParamModes.Queue:
                    _songs.Shuffle();
                    return;
                default:
                    return;
            }
        }

        public Tuple<Song?, List<Song>, List<Song>> GetQueue()
        {
            return Tuple.Create(_nowPlaying, _songsPrio, _songs);
        }

        private async void OnErrorEvent(string message)
        {
            await _msgChannel.SendMessageAsync(message);
        }
    }
}
