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
        string text = File.ReadAllText(inputPath);

        // regex
        var phoneRegex = new Regex(@"\+395\s?\d{3}\s?\d{2}\s?\d{2}");
        var idRegex = new Regex(@"\b\d{6}\b");
        var nameRegex = new Regex(@"[А-Яа-я]+");

        var matches = new List<(int index, string type, string value)>();

        foreach (Match m in phoneRegex.Matches(text))
            matches.Add((m.Index, "phone", NormalizePhone(m.Value)));

        foreach (Match m in idRegex.Matches(text))
            matches.Add((m.Index, "id", m.Value));

        foreach (Match m in nameRegex.Matches(text))
            matches.Add((m.Index, "name", m.Value));

        // sorting
        matches.Sort((a, b) => a.index.CompareTo(b.index));

        List<Contact> contacts = new List<Contact>();

        string name = null, id = null, phone = null;

        foreach (var item in matches)
        {
            if (item.type == "name" && name == null)
                name = item.value;

            else if (item.type == "id" && id == null)
                id = item.value;

            else if (item.type == "phone" && phone == null)
                phone = item.value;

            if (name != null && id != null && phone != null)
            {
                contacts.Add(new Contact
                {
                    Name = name,
                    Id = id,
                    Phone = phone
                });

                name = id = phone = null;
            }
        }

        // XML
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