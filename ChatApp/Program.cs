using System;

class Program
{
    static void Main()
    {
        var room = new ChatRoom("TestRoom");

        var user1 = new Contact("Ivan");
        var user2 = new Contact(null); // ще генерира име

        room.AddUser(user1);
        room.AddUser(user2);

        room.AddMessage(new Message(user1, "Hello!"));
        room.AddMessage(new Message(user2, "Hi there!"));

        room.Messages[1].Edit("Hi there!!!");

        room.PrintMessages();

        Console.WriteLine("\nMessages by Ivan:");
        foreach (var msg in room.GetMessagesByUser("Ivan"))
            Console.WriteLine(msg);
    }
}