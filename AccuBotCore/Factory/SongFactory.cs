using AccuBotCore.Models;

namespace AccuBotCore.Factory
{
    public class SongFactory
    {
        public Song Create(Video video)
        {
            return CreateInternal(video.Title, video.Url);
        }

        private Song CreateInternal(string title, string url)
        {
            return new Song(title, url);
        }
    }
}
