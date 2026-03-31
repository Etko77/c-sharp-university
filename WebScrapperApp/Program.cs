using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebScraperApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Споделен HttpClient — всяка задача сама слага нужните headers
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(20);

            // Инициализираме всяка задача с общия клиент
            var task1 = new WhoisTask(httpClient);
            var task2 = new CurrentTimeTask(httpClient);
            var task3 = new NewsScraperTask(httpClient);

            bool running = true;
            while (running)
            {
                Console.Clear();
                PrintMenu();
                Console.Write("Избор: ");
                string? choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        await task1.Run();
                        break;
                    case "2":
                        await task2.Run();
                        break;
                    case "3":
                        await task3.Run();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Довиждане!");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Невалиден избор.");
                        Console.ResetColor();
                        break;
                }

                if (running && choice != "0")
                {
                    Console.WriteLine("\nНатиснете Enter за връщане към менюто...");
                    Console.ReadLine();
                }
            }
        }

        static void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║         Web Scraper – C# Console         ║");
            Console.WriteLine("╠══════════════════════════════════════════╣");
            Console.WriteLine("║  1.  WHOIS – IP адрес → Държава          ║");
            Console.WriteLine("║  2.  Текущ час в София                    ║");
            Console.WriteLine("║  3.  Новини от Mediapool (без COVID)      ║");
            Console.WriteLine("║  0.  Изход                                ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}