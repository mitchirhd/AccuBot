using AccuBotCore.Jobs;
using AccuBotCore.Provider;
using Discord;
using Discord.Audio;

namespace AccuBotCore.Factory
{
    public class SongPlayerJobFactory
    {
        IYoutubeInfoProvider _youtubeInfoProvider;

        public SongPlayerJobFactory(IYoutubeInfoProvider infoProvider)
        {
            _youtubeInfoProvider = infoProvider;
        }

        public SongPlayerJob CreateSongPlayerJob(IAudioClient audioClient, IMessageChannel messageChannel)
        {
            return new SongPlayerJob(audioClient, messageChannel, _youtubeInfoProvider);
        }
    }
}
