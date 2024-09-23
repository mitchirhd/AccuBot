using AccuBotCore.Models;
using Discord;

namespace AccuBotCore.Factory
{
    public class QueueStringFactory
    {
        public string Create(Tuple<Song, List<Song>, List<Song>>? entries)
        {
            var returnValue = "";
            if (entries == null)
                return returnValue;

            int maxLines = 10;
            returnValue += CreateNowPlayíng(entries.Item1) + "\n";
            if (entries.Item2.Count > 0)
                returnValue += $"{Format.Bold("Priority queue")}\n" + HandleList(entries.Item2, maxLines, 0) + "\n\n";
            if (entries.Item3.Count > 0)
                returnValue += $"{Format.Bold("Queue")}\n" + HandleList(entries.Item3, maxLines, Math.Min(entries.Item2.Count + 1, maxLines));

            return returnValue;
        }

        public string CreateNowPlayíng(Song song)
            => $"Now playing: {Format.Bold(song.Name)}\n\n";

        private string HandleList(List<Song> entries, int maxLines, int offset) 
        {
            var returnValue = "";

            for(int i = 0; i < Math.Min(maxLines, entries.Count); i++)
            {
                var song = entries[i];
                returnValue += $"{i + offset}. {song.Name}\n";
            }
            if (entries.Count > maxLines)
                returnValue += "...";

            return returnValue;
        }
    }
}
