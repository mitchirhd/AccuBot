using Discord;

namespace AccuBotCore.Factory
{
    public class HelpStringFactory
    {
        public string Create()
        {
            var returnValue = "";

            returnValue += "play <Youtube URL> - tut ein Video ins Queue laden - p\n";
            returnValue += "playnext <Youtube URL> - tut ein Video ins Priority Queue laden - pn\n";
            returnValue += "skip - tut ein Lied übersprungen\n";
            returnValue += $"clear <{Format.Bold("all")}, prio, queue> - macht das Queue leer\n";
            returnValue += "leave - macht den Bot rauschmeisn und Queue clearn\n";
            returnValue += $"shuffle <{Format.Bold("all")}, prio, queue> - macht die Queue durcheinander\n";
            returnValue += "queue - gibt das Kuh aus - q\n";
            returnValue += "nowplaying - sagt was grad gespielt wird - np\n";
            returnValue += "help - kek\n";

            return returnValue;
        }
    }
}
