namespace WebScraperApp
{
    public class NewsArticle
    {
        public string Title    { get; init; } = "";
        public string Date     { get; init; } = "";
        public string Time     { get; init; } = "";

        public string DateTimeDisplay =>
            string.IsNullOrWhiteSpace(Time) ? Date : $"{Date} {Time}".Trim();
    }
}
