using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScraperApp
{
    public class NewsScraperTask
    {
        private readonly HttpClient _httpClient;
        private const string SiteUrl    = "https://www.mediapool.bg/";
        private const string OutputFile = "news_mediapool.json";

        private static readonly string[] BlockedKeywords =
        {
            "covid-19", "covid", "corona", "корона вирус", "коронавирус",
            "coronavirus", "пандемия", "pandemic"
        };

        public NewsScraperTask(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== ЗАДАЧА 3: Новини от Mediapool ===");
            Console.ResetColor();
            Console.WriteLine("\nИзтегляне на " + SiteUrl + " ...");

            try
            {
                string html = await FetchHtml(SiteUrl);
                var articles = ParseArticles(html);

                if (articles.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Не бяха намерени статии.");
                    Console.ResetColor();
                    return;
                }

                PrintArticles(articles);
                SaveToJson(articles);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nГрешка: " + ex.Message);
                Console.ResetColor();
            }
        }

        private async Task<string> FetchHtml(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/124.0.0.0 Safari/537.36");
            request.Headers.TryAddWithoutValidation("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Headers.TryAddWithoutValidation("Accept-Language",
                "bg,en-US;q=0.9,en;q=0.8");
            request.Headers.TryAddWithoutValidation("Connection", "keep-alive");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Структура на Mediapool:
        //   <article>
        //     <h3 class="c-article-item__title">Заглавие</h3>
        //     <time datetime="..." class="c-article-item__date">14:38</time>
        //   </article>
        //
        // Стратегия: намираме всички <article> тагове,
        // после в тях търсим <h3> за заглавие и <time> за час.
        private static List<NewsArticle> ParseArticles(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var articles = new List<NewsArticle>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Намираме всички <article> тагове на страницата
            var articleNodes = doc.DocumentNode.SelectNodes("//article");
            if (articleNodes == null) return articles;

            foreach (var articleNode in articleNodes)
            {
                // Заглавие: първия <h3> или <h2> в article-а
                var titleNode = articleNode.SelectSingleNode(".//h3")
                             ?? articleNode.SelectSingleNode(".//h2");

                if (titleNode == null) continue;

                string title = HtmlEntity.DeEntitize(titleNode.InnerText).Trim();
                if (string.IsNullOrWhiteSpace(title)) continue;
                if (title.Length < 10) continue;
                if (!seen.Add(title)) continue;
                if (ContainsBlockedKeyword(title)) continue;

                // Дата/час: първия <time> в article-а
                string datetime = "—";
                var timeNode = articleNode.SelectSingleNode(".//time");
                if (timeNode != null)
                {
                    string inner = HtmlEntity.DeEntitize(timeNode.InnerText).Trim();
                    datetime = string.IsNullOrWhiteSpace(inner)
                        ? timeNode.GetAttributeValue("datetime", "—")
                        : inner;
                }

                articles.Add(new NewsArticle
                {
                    Title = title,
                    Date  = datetime,
                    Time  = ""
                });
            }

            return articles;
        }

        private static bool ContainsBlockedKeyword(string title)
        {
            string lower = title.ToLowerInvariant();
            foreach (string kw in BlockedKeywords)
                if (lower.Contains(kw)) return true;
            return false;
        }

        private static void PrintArticles(List<NewsArticle> articles)
        {
            int displayed = Math.Min(articles.Count, 20);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Намерени: " + articles.Count + "  |  Показани: " + displayed);
            Console.WriteLine("(Изключени: covid, коронавирус, пандемия)");
            Console.WriteLine(new string('=', 70));
            Console.ResetColor();

            for (int i = 0; i < displayed; i++)
            {
                var a = articles[i];
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("  [" + (i + 1).ToString().PadLeft(2) + "] ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(a.Title);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("       " + a.DateTimeDisplay);
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        private static void SaveToJson(List<NewsArticle> articles)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("[");
                for (int i = 0; i < articles.Count; i++)
                {
                    var a    = articles[i];
                    string t = a.Title.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    string d = a.Date.Replace("\"", "\\\"");
                    sb.Append("  { \"title\": \"" + t + "\", \"datetime\": \"" + d + "\" }");
                    if (i < articles.Count - 1) sb.Append(",");
                    sb.AppendLine();
                }
                sb.AppendLine("]");
                File.WriteAllText(OutputFile, sb.ToString(), Encoding.UTF8);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Записано в: " + Path.GetFullPath(OutputFile));
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("(Грешка при запис: " + ex.Message + ")");
                Console.ResetColor();
            }
        }
    }
}