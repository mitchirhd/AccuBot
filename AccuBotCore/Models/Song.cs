namespace AccuBotCore.Models
{
    public class Song
    {
        public Song(string name, string url)
        {
            Name = name;
            URL = url;
        }

        public string Name { get; set; }

        public string URL { get; set; }
    }
}
