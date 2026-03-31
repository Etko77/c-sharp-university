using System;
using System.Net.Http;
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
            Console.WriteLine("═══ ЗАДАЧА 1: IP → Държава (WHOIS) ═══");
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

                Console.WriteLine($"\nТърсене на информация за: {ip}");
                Console.WriteLine(new string('─', 50));

                await QuerySite1(ip);
                await QuerySite2(ip);

                Console.WriteLine();
            }
        }

        private async Task QuerySite1(string ip)
        {
            try
            {
                string url = $"https://progress.razorlabs.com/ip-detect/?ip={ip}";
                Console.Write("  Сайт 1 (razorlabs.com) : ");
                string result = await _httpClient.GetStringAsync(url);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(result.Trim());
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Грешка: {ex.Message}");
                Console.ResetColor();
            }
        }

        private async Task QuerySite2(string ip)
        {
            try
            {
                string url = $"https://ipapi.co/{ip}/country/";
                Console.Write("  Сайт 2 (ipapi.co)      : ");
                string result = await _httpClient.GetStringAsync(url);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(result.Trim());
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Грешка: {ex.Message}");
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
