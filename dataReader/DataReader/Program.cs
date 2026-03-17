using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        //EXERCISE 1 TEST

        // LocationService.ProcessFile(
        //     @"C:\Users\etien\Desktop\csharp\dataReader\DataReader\input-01.txt",
        //     "output.json"
        // );
        // Console.WriteLine(File.Exists("input-01.txt"));

        //EXERCISE 2 TEST

        ContactService.ProcessFile("input-02.txt", "contacts.xml");
    }
}

