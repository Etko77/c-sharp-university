using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        LocationService.ProcessFile(
            @"C:\Users\etien\Desktop\csharp\dataReader\DataReader\input-01.txt",
            "output.json"
        );
        Console.WriteLine(File.Exists("input-01.txt"));
    }
}

