using AccuBotCore.Models;

namespace AccuBotCore.Provider
{
    public delegate void YoutubeInfoProviderErrorEvent(string message);

    public interface IYoutubeInfoProvider
    {

        event YoutubeInfoProviderErrorEvent? ErrorEvent;

        Task<Stream> GetAudioStreamAsync(string url);

        Task<Video> GetVideoInfo(string url);

        Task<List<Video>> GetPlaylistVideos(string url);
    }
}
