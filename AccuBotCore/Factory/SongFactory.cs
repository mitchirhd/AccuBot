using AccuBotCore.Models;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace AccuBotCore.Factory
{
    public class SongFactory
    {
        public Song Create(Video video)
        {
            return CreateInternal(video.Title, video.Url);
        }

        public Song Create(PlaylistVideo playlistVideo)
        {
            return CreateInternal(playlistVideo.Title, playlistVideo.Url);
        }

        private Song CreateInternal(string title, string url)
        {
            return new Song(title, url);
        }
    }
}
