using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Globalization;

public class LocationService
{
    public static void ProcessFile(string inputPath, string outputPath)
    {
        string input = File.ReadAllText(inputPath);

        List<Location> locations = input
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair =>
            {
                var parts = pair.Split(',');
                return new Location
                {
                    lat = float.Parse(parts[0], CultureInfo.InvariantCulture),
                    lng = float.Parse(parts[1], CultureInfo.InvariantCulture)
                };
            })
            .ToList();

        string json = JsonSerializer.Serialize(locations);

        File.WriteAllText(outputPath, json);
    }
    
}
public struct Location
{
    public float lat { get; set; }
    public float lng { get; set; }
}