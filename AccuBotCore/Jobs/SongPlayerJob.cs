using AccuBotCore.Enum;
using AccuBotCore.Models;
using CliWrap;
using Discord.Audio;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace AccuBotCore.Jobs
{
    public delegate void PlayingSong(SongPlayerJob job, Video videoInfo);

    public class SongPlayerJob : JobBase
    {
        private readonly IAudioClient _audioClient;
        private List<Song> _songs = [];
        private List<Song> _songsPrio = [];
        private Task? _currentTask;
        private bool _stopCurrent;
        private bool _isPlaying;
        private YoutubeClient _ytClient;
        private Song? _nowPlaying = null;

        public event PlayingSong? PlayingSongEvent;

        public SongPlayerJob(IAudioClient audioClient, YoutubeClient ytClient) 
        {
            _audioClient = audioClient;
            _ytClient = ytClient;
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

            return PlaySong(_audioClient, prio);
        }

        private async Task PlaySong(IAudioClient AudioClient, bool prio)
        {
            if (_currentTask != null && !_currentTask.IsCompleted)
                _currentTask.Wait();

            _isPlaying = true;
            _stopCurrent = false;

            _nowPlaying = prio ? _songsPrio.First() : _songs.First();
            string url = _nowPlaying.URL;
            StreamManifest streamManifest = await _ytClient.Videos.Streams.GetManifestAsync(url);
            IStreamInfo streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            Stream InputStream = await _ytClient.Videos.Streams.GetAsync(streamInfo);

            MemoryStream memory = new MemoryStream();
            await Cli.Wrap("ffmpeg").WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                                    .WithStandardInputPipe(PipeSource.FromStream(InputStream))
                                    .WithStandardOutputPipe(PipeTarget.ToStream(memory))
                                    .ExecuteAsync();
            _currentTask = Task.Run(() =>
            {
                if (prio)
                {
                    if (_songsPrio.Count > 0)
                        _songsPrio.RemoveAt(0);
                }
                else if (_songs.Count > 0)
                    _songs.RemoveAt(0);

                Task.Run(async () => PlayingSongEvent?.Invoke(this, await _ytClient.Videos.GetAsync(url)));
                memory.Seek(0, SeekOrigin.Begin);
                using (AudioOutStream outputStream = AudioClient.CreatePCMStream(AudioApplication.Mixed))
                {
                    int readBytes = 0;
                    byte[] buffer = new byte[4096];
                    while (!_stopCurrent && (readBytes = memory.Read(buffer, 0, buffer.Length)) > 0)
                        outputStream.Write(buffer, 0, readBytes);
                }
            });
            await _currentTask;
            _isPlaying = false;
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
    }
}
