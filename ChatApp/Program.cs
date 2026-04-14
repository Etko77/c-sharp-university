using System;
using System.Linq;

class Program
{
    static void Main()
    {
        var room = new ChatRoom("MyRoom");

        while (true)
        {
            Console.WriteLine("\n=== MENU ===");
            Console.WriteLine("1. Add user");
            Console.WriteLine("2. Send message");
            Console.WriteLine("3. Show all messages");
            Console.WriteLine("4. Show messages by user");
            Console.WriteLine("5. Show all users");
            Console.WriteLine("6. Show longest message");
            Console.WriteLine("7. Show shortest message");
            Console.WriteLine("8. Show messages sorted by HEX");
            Console.WriteLine("0. Exit");

            Console.Write("Choice: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddUser(room);
                    break;

                case "2":
                    AddMessage(room);
                    break;

                case "3":
                    ShowMessages(room);
                    break;

                case "4":
                    ShowMessagesByUser(room);
                    break;

                case "5":
                    ShowUsers(room);
                    break;

                case "6":
                    var longest = room.GetLongestMessage();
                    Console.WriteLine(longest != null ? longest : "No messages");
                    break;

                case "7":
                    var shortest = room.GetShortestMessage();
                    Console.WriteLine(shortest != null ? shortest : "No messages");
                    break;

                case "8":
                    foreach (var m in room.GetMessagesSortedByHex())
                        Console.WriteLine(m);
                    break;

                case "0":
                    return;

                case string s when string.IsNullOrWhiteSpace(s):
                    Console.WriteLine("Empty input!");
                    break;

                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }
    }

    static void AddUser(ChatRoom room)
    {
        Console.Write("Enter username: ");
        var name = Console.ReadLine() ?? "";

        var user = new Contact(name);
        room.AddUser(user);
    }

    static void AddMessage(ChatRoom room)
    {
        Console.Write("Username: ");
        var name = Console.ReadLine() ?? "";

        var user = room.Users
            .FirstOrDefault(u => u.Name == name);

        if (user == null)
        {
            user = new Contact(name);
            room.AddUser(user);
        }

        Console.Write("Message: ");
        var text = Console.ReadLine() ?? "";

        room.AddMessage(new Message(user, text));
    }

    static void ShowMessages(ChatRoom room)
    {
        if (!room.Messages.Any())
        {
            Console.WriteLine("No messages.");
            return;
        }

        room.Messages.ForEach(m => Console.WriteLine(m));
    }

    static void ShowMessagesByUser(ChatRoom room)
    {
        Console.Write("Username: ");
        var name = Console.ReadLine() ?? "";

        var messages = room.GetMessagesByUser(name);

        foreach (var m in messages)
            Console.WriteLine(m);
    }

    static void ShowUsers(ChatRoom room)
    {
        if (!room.Users.Any())
        {
            Console.WriteLine("No users.");
            return;
        }

        room.Users.ForEach(u => Console.WriteLine(u));
    }
}