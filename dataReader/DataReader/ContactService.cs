using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Globalization;

public class ContactService
{
    public static void ProcessFile(string inputPath, string outputPath)
{
    var lines = File.ReadAllLines(inputPath);
    var contacts = new List<Contact>();

    foreach (var line in lines)
    {
        var match = Regex.Match(line, @"(?<name>[А-Яа-я]+)\s+(?<id>\d{6})\s+(?<phone>\+395\s?\d{3}\s?\d{2}\s?\d{2})");
        if (match.Success)
        {
            contacts.Add(new Contact
            {
                Name = match.Groups["name"].Value,
                Id = match.Groups["id"].Value,
                Phone = match.Groups["phone"].Value.Replace(" ", "")
            });
        }
    }

    XmlSerializer serializer = new XmlSerializer(typeof(List<Contact>));
    using var fs = new FileStream(outputPath, FileMode.Create);
    serializer.Serialize(fs, contacts);
}

    private static string NormalizePhone(string phone)
    {
        return phone.Replace(" ", "");
    }
}

public class Contact
{
    public string Name { get; set; } = "";
    public string Id { get; set; } = "";
    public string Phone { get; set; } = "";
}