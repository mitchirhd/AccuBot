using AccuBotCore.Controller;
using AccuBotCore.Models;
using Discord;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace AccuBotCore.Provider
{
    public class YoutubeExplodeInfoProvider : IYoutubeInfoProvider
    {
        private YoutubeClient _ytClient;

        public event YoutubeInfoProviderErrorEvent? ErrorEvent;

        public YoutubeExplodeInfoProvider()
        {
            _ytClient = new YoutubeClient();
        }

        public async Task<Stream> GetAudioStreamAsync(string url)
        {
            try
            {
                StreamManifest streamManifest = await _ytClient.Videos.Streams.GetManifestAsync(url);
                IStreamInfo streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                return await _ytClient.Videos.Streams.GetAsync(streamInfo);
            }
            catch (Exception ex)
            {
                ErrorEvent?.Invoke($"Could not get video information: {ex.Message}");
                Logger.Log(new LogMessage(LogSeverity.Critical, "SongPlayerJob", "Error while requesting stream info", ex));
                return Stream.Null;
            }
        }

        public async Task<Video> GetVideoInfo(string url)
        {
            var explodeVideo = await _ytClient.Videos.GetAsync(url);
            Video video = new Video(explodeVideo.Title, explodeVideo.Url);
            return video;
        }

        public async Task<List<Video>> GetPlaylistVideos(string url)
        {
            var explodeVideos = await _ytClient.Playlists.GetVideosAsync(url);
            var videos = new List<Video>();
            foreach (var explodeVideo in explodeVideos)
            {
                videos.Add(new Video(explodeVideo.Title, explodeVideo.Url));
            }
            return videos;
        }
    }
}
