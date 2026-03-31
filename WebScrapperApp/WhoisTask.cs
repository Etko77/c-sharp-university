using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebScraperApp
{
    public class WhoisTask
    {
        private readonly HttpClient _httpClient;

        public WhoisTask(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ ЗАДАЧА 1: IP → Информация (ipinfo.io) ═══");
            Console.ResetColor();

            while (true)
            {
                Console.Write("\nВъведете IPv4 адрес (или 'back' за изход): ");
                string? ip = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(ip) || ip.ToLower() == "back")
                    break;

                if (!IsValidIPv4(ip))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Невалиден IPv4 адрес. Формат: 0-255.0-255.0-255.0-255");
                    Console.ResetColor();
                    continue;
                }

                await QueryIpInfo(ip);
                Console.WriteLine();
            }
        }

        private async Task QueryIpInfo(string ip)
        {
            try
            {
                string url = $"https://ipinfo.io/{ip}/json";

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.TryAddWithoutValidation("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                    "AppleWebKit/537.36 (KHTML, like Gecko) " +
                    "Chrome/124.0.0.0 Safari/537.36");
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("Accept-Language", "bg,en;q=0.9");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                Console.WriteLine($"\nРезултат за: {ip}");
                Console.WriteLine(new string('─', 50));

                PrintField(root, "ip",       "IP адрес   ");
                PrintField(root, "hostname", "Hostname   ");
                PrintField(root, "city",     "Град       ");
                PrintField(root, "region",   "Регион     ");
                PrintField(root, "country",  "Държава    ");
                PrintField(root, "org",      "Оператор   ");
                PrintField(root, "timezone", "Часова зона");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Грешка: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void PrintField(JsonElement root, string key, string label)
        {
            if (root.TryGetProperty(key, out var val))
            {
                string value = val.GetString() ?? "—";
                Console.Write($"  {label} : ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(value);
                Console.ResetColor();
            }
        }

        private static bool IsValidIPv4(string ip)
        {
            string[] parts = ip.Split('.');
            if (parts.Length != 4) return false;
            foreach (string part in parts)
            {
                if (!int.TryParse(part, out int num)) return false;
                if (num < 0 || num > 255) return false;
            }
            return true;
        }
    }
}