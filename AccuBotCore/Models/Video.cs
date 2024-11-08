namespace AccuBotCore.Models
{
    public class Video
    {
        public Video(string title, string url)
        {
            Title = title;
            Url = url;
        }

        public string Title { get; set; }

        public string Url { get; set; }
    }
}
