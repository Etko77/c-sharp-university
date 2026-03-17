using System;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using System.Diagnostics;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        EnsureFiles();
        while (true)
        {
            Console.WriteLine("Избери програма:");
            Console.WriteLine("1 - Показване на карта с координати");
            Console.WriteLine("2 - Търсене на телефон по име или ID");
            Console.WriteLine("0 - Изход");
            Console.Write("Въведи избор: ");

            string choice = Console.ReadLine();

            if (choice == "0")
                break;

            switch (choice)
            {
                case "1":
                    ShowMap();
                    break;
                case "2":
                    SearchContact();
                    break;
                default:
                    Console.WriteLine("Невалиден избор!");
                    break;
            }
        }
    }
    static void EnsureFiles()
    {

        // contacts.xml
        if (!File.Exists("contacts.xml"))
        {
            Console.WriteLine("Генериране на contacts.xml...");
            ContactService.ProcessFile("input-01.txt", "contacts.xml");
        }

        // output.json
        if (!File.Exists("output.json"))
        {
            Console.WriteLine("Генериране на output.json...");
            LocationService.ProcessFile("input-02.txt", "output.json");
        }
    }

    static void ShowMap()
    {
        string jsonPath = "output.json";
        string htmlPath = "map.html";

        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"Файлът {jsonPath} не съществува. Моля, стартирай LocationService първо.");
            return;
        }

        var locationsJson = File.ReadAllText(jsonPath);
        File.WriteAllText(htmlPath, GenerateHtml(locationsJson));

        Console.WriteLine("Отваряне на картата в браузъра...");
        Process.Start(new ProcessStartInfo
        {
            FileName = htmlPath,
            UseShellExecute = true
        });
    }

    static string GenerateHtml(string locationsJson)
    {
        return $@"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"" />
  <title>Map</title>
  <link rel=""stylesheet"" href=""https://unpkg.com/leaflet/dist/leaflet.css"" />
  <style>html, body {{ height: 100%; margin: 0; }} #map {{ height: 100%; }}</style>
</head>
<body>
  <div id=""map""></div>
  <script src=""https://unpkg.com/leaflet/dist/leaflet.js""></script>
  <script>
    var locations = {locationsJson};
    var map = L.map('map').setView([50.0679994, 21.5529823], 6);
    L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
      attribution: '© OpenStreetMap contributors'
    }}).addTo(map);
    locations.forEach(loc => L.marker([loc.lat, loc.lng]).addTo(map));
  </script>
</body>
</html>";
    }

    static void SearchContact()
    {
        string xmlPath = "contacts.xml";

        if (!File.Exists(xmlPath))
        {
            Console.WriteLine($"Файлът {xmlPath} не съществува. Моля, стартирай ContactService първо.");
            return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(List<Contact>));
        List<Contact> contacts;
        using (var fs = new FileStream(xmlPath, FileMode.Open))
        {
            contacts = (List<Contact>)serializer.Deserialize(fs);
        }

        Console.Write("Въведи име или ID: ");
        string input = Console.ReadLine();
        Console.WriteLine(input);
        var contact = contacts.Find(c => c.Name.Equals(input, StringComparison.CurrentCultureIgnoreCase)
                                        || c.Id == input);

        if (contact != null)
            Console.WriteLine($"Телефонният номер на {contact.Name} е: {contact.Phone}");
        else
            Console.WriteLine("Не е намерен контакт с това име или ID.");
    }
}
