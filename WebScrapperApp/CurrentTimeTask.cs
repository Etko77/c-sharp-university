using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScraperApp
{
    public class CurrentTimeTask
    {
        private readonly HttpClient _httpClient;
        private const string Url = "https://www.timeanddate.com/worldclock/bulgaria/sofia";

        public CurrentTimeTask(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ ЗАДАЧА 2: Текущ час в София ═══");
            Console.ResetColor();
            Console.WriteLine("\nИзтегляне на страницата от timeanddate.com...");

            try
            {
                string html = await _httpClient.GetStringAsync(Url);
                var (time, date) = ParseTimeAndDate(html);
                PrintResult(time, date);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nГрешка при изтегляне: {ex.Message}");
                Console.ResetColor();
                PrintSystemFallback();
            }
        }

        // Парсва HTML-а и връща (час, дата) като tuple
        private static (string time, string date) ParseTimeAndDate(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            string time = "неизвестно";
            string date = "неизвестно";

            // Опит 1: <span id="ct"> – статичен часовник
            var timeNode = doc.DocumentNode.SelectSingleNode("//*[@id='ct']");
            if (timeNode != null && !string.IsNullOrWhiteSpace(timeNode.InnerText))
                time = HtmlEntity.DeEntitize(timeNode.InnerText).Trim();

            // Опит 2: <span id="ctdat"> – дата
            var dateNode = doc.DocumentNode.SelectSingleNode("//*[@id='ctdat']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
                date = HtmlEntity.DeEntitize(dateNode.InnerText).Trim();

            // Опит 3: <span id="bdc"> – алтернативен дата елемент
            if (date == "неизвестно")
            {
                var bdcNode = doc.DocumentNode.SelectSingleNode("//*[@id='bdc']");
                if (bdcNode != null && !string.IsNullOrWhiteSpace(bdcNode.InnerText))
                    date = HtmlEntity.DeEntitize(bdcNode.InnerText).Trim();
            }

            return (time, date);
        }

        private static void PrintResult(string time, string date)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("┌──────────────────────────────────┐");
            Console.WriteLine("│   Текущ час – София, България    │");
            Console.WriteLine("├──────────────────────────────────┤");
            Console.WriteLine($"│  Дата : {date,-25}│");
            Console.WriteLine($"│  Час  : {time,-25}│");
            Console.WriteLine("└──────────────────────────────────┘");
            Console.ResetColor();

            // Ако JS е зарендил часовника динамично, стойностите ще са празни
            if (time == "неизвестно" || date == "неизвестно")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\nЗабележка: timeanddate.com зарежда часовника чрез JavaScript.");
                Console.WriteLine("При статично HTTP парсване стойностите могат да са празни.");
                Console.WriteLine("Използваме системния часовник като fallback:");
                Console.ResetColor();
                PrintSystemFallback();
            }
        }

        // Fallback: изчисляваме времето от системния UTC
        private static void PrintSystemFallback()
        {
            try
            {
                // Linux/Mac timezone ID
                var sofiaTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                    DateTime.UtcNow, "Europe/Sofia");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  Дата  : {sofiaTime:dddd, dd MMMM yyyy}");
                Console.WriteLine($"  Час   : {sofiaTime:HH:mm:ss} EET");
                Console.ResetColor();
            }
            catch
            {
                // Windows timezone ID
                try
                {
                    var sofiaTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                        DateTime.UtcNow, "FLE Standard Time");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n  Дата  : {sofiaTime:dddd, dd MMMM yyyy}");
                    Console.WriteLine($"  Час   : {sofiaTime:HH:mm:ss} EET");
                    Console.ResetColor();
                }
                catch
                {
                    // Краен fallback: UTC+2
                    var utcPlus2 = DateTime.UtcNow.AddHours(2);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n  Дата  : {utcPlus2:dddd, dd MMMM yyyy}");
                    Console.WriteLine($"  Час   : {utcPlus2:HH:mm:ss} UTC+2");
                    Console.ResetColor();
                }
            }
        }
    }
}
