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
        private const string SiteUrl = "https://www.mediapool.bg/";
        private const string OutputFile = "news_mediapool.json";

        // Ключови думи за изключване (case-insensitive)
        private static readonly string[] BlockedKeywords =
        {
            "covid-19", "covid", "корона вирус", "коронавирус",
            "coronavirus", "пандемия", "pandemic"
        };

        public NewsScraperTask(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ ЗАДАЧА 3: Новини от Mediapool ═══");
            Console.ResetColor();
            Console.WriteLine($"\nИзтегляне на {SiteUrl} ...");

            try
            {
                string html = await _httpClient.GetStringAsync(SiteUrl);
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
                Console.WriteLine($"\nГрешка: {ex.Message}");
                Console.ResetColor();
            }
        }

        // ── Парсване ─────────────────────────────────────────────────
        private static List<NewsArticle> ParseArticles(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var articles = new List<NewsArticle>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Стратегия 1: намираме <article> тагове
            var nodes = doc.DocumentNode.SelectNodes("//article");

            // Стратегия 2: div-ове с "news" или "article" в класа
            if (nodes == null || nodes.Count == 0)
            {
                nodes = doc.DocumentNode.SelectNodes(
                    "//div[contains(@class,'news') or contains(@class,'article')]");
            }

            // Стратегия 3: всички <a> с достатъчно дълъг текст
            if (nodes == null || nodes.Count == 0)
            {
                return FallbackLinkParse(doc, seen);
            }

            foreach (var node in nodes)
            {
                string title = ExtractTitle(node);
                if (string.IsNullOrWhiteSpace(title)) continue;
                if (!seen.Add(title)) continue;
                if (ContainsBlockedKeyword(title)) continue;

                var (date, time) = ExtractDateTime(node);

                articles.Add(new NewsArticle
                {
                    Title = title,
                    Date  = date,
                    Time  = time
                });
            }

            // Ако статегии 1/2 са намерили нещо, но без заглавия – fallback
            if (articles.Count == 0)
                return FallbackLinkParse(doc, seen);

            return articles;
        }

        private static string ExtractTitle(HtmlNode node)
        {
            // Приоритет: h1 > h2 > h3 > h4 > дълъг <a>
            foreach (string xpath in new[] { ".//h1", ".//h2", ".//h3", ".//h4" })
            {
                var h = node.SelectSingleNode(xpath);
                if (h != null && !string.IsNullOrWhiteSpace(h.InnerText))
                    return HtmlEntity.DeEntitize(h.InnerText).Trim();
            }

            var link = node.SelectSingleNode(
                ".//a[string-length(normalize-space(text())) > 20]");
            if (link != null && !string.IsNullOrWhiteSpace(link.InnerText))
                return HtmlEntity.DeEntitize(link.InnerText).Trim();

            return "";
        }

        private static (string date, string time) ExtractDateTime(HtmlNode node)
        {
            // Търсим <time> елемент
            var timeNode = node.SelectSingleNode(".//time");
            if (timeNode != null)
            {
                string inner = HtmlEntity.DeEntitize(timeNode.InnerText).Trim();
                if (!string.IsNullOrWhiteSpace(inner))
                    return SplitDateTime(inner);

                // datetime атрибут
                string attr = timeNode.GetAttributeValue("datetime", "");
                if (!string.IsNullOrWhiteSpace(attr))
                    return (attr, "");
            }

            // Търсим span/div с "date" или "time" в класа
            var dtNode = node.SelectSingleNode(
                ".//*[contains(@class,'date') or contains(@class,'time')]");
            if (dtNode != null && !string.IsNullOrWhiteSpace(dtNode.InnerText))
                return SplitDateTime(HtmlEntity.DeEntitize(dtNode.InnerText).Trim());

            return ("—", "");
        }

        // Опитва се да раздели "01.03.2025 14:30" на дата + час
        private static (string date, string time) SplitDateTime(string raw)
        {
            string[] parts = raw.Split(new[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return (parts[0], parts[1]);
            return (raw, "");
        }

        // Fallback: намираме всички <a> с дълги заглавия
        private static List<NewsArticle> FallbackLinkParse(
            HtmlDocument doc, HashSet<string> seen)
        {
            var articles = new List<NewsArticle>();

            var links = doc.DocumentNode.SelectNodes(
                "//a[string-length(normalize-space(text())) > 25]");

            if (links == null) return articles;

            foreach (var link in links)
            {
                string title = HtmlEntity.DeEntitize(link.InnerText).Trim();
                if (string.IsNullOrWhiteSpace(title)) continue;
                if (!seen.Add(title)) continue;
                if (ContainsBlockedKeyword(title)) continue;

                // Търсим дата в родителски елементи (до 4 нива нагоре)
                string date = "—";
                var parent = link.ParentNode;
                for (int i = 0; i < 4 && parent != null; i++)
                {
                    var t = parent.SelectSingleNode(".//time");
                    if (t != null)
                    {
                        date = HtmlEntity.DeEntitize(t.InnerText).Trim();
                        if (string.IsNullOrWhiteSpace(date))
                            date = t.GetAttributeValue("datetime", "—");
                        break;
                    }
                    parent = parent.ParentNode;
                }

                articles.Add(new NewsArticle { Title = title, Date = date });
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

        // ── Извеждане ────────────────────────────────────────────────
        private static void PrintArticles(List<NewsArticle> articles)
        {
            int displayed = Math.Min(articles.Count, 20);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Намерени статии : {articles.Count}  |  Показани : {displayed}");
            Console.WriteLine("(Изключени статии с: covid, коронавирус, пандемия)");
            Console.WriteLine(new string('═', 72));
            Console.ResetColor();

            for (int i = 0; i < displayed; i++)
            {
                var a = articles[i];
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"  [{i + 1,2}] ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(a.Title);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"       📅 {a.DateTimeDisplay}");
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        // ── Запис в JSON ─────────────────────────────────────────────
        private static void SaveToJson(List<NewsArticle> articles)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("[");
                for (int i = 0; i < articles.Count; i++)
                {
                    var a = articles[i];
                    string title = a.Title.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    string date  = a.Date.Replace("\"", "\\\"");
                    string time  = a.Time.Replace("\"", "\\\"");

                    sb.Append($"  {{ \"title\": \"{title}\", " +
                              $"\"date\": \"{date}\", " +
                              $"\"time\": \"{time}\" }}");
                    if (i < articles.Count - 1) sb.Append(",");
                    sb.AppendLine();
                }
                sb.AppendLine("]");

                File.WriteAllText(OutputFile, sb.ToString(), Encoding.UTF8);

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"✔  Записано в: {Path.GetFullPath(OutputFile)}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"(Неуспешен запис на JSON: {ex.Message})");
                Console.ResetColor();
            }
        }
    }
}
