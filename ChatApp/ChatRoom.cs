using System;
using System.Collections.Generic;
using System.Linq;

public class ChatRoom
{
    public string Name { get; set; }
    public List<Contact> Users { get; set; } = new();
    public List<Message> Messages { get; set; } = new();

    public ChatRoom(string? name) =>
        Name = name ?? "DefaultRoom";

    public void AddUser(Contact? user)
    {
        var exists = Users.Any(u => u.Name == user?.Name);

        if (!exists)
            Users.Add(user ?? new Contact(null));
        else
            Console.WriteLine("User with this name already exists!");
    }

    public void AddMessage(Message? msg) =>
        Messages.Add(msg ?? new Message(null, null));

    public IEnumerable<Message> GetMessagesByUser(string? username) =>
        Messages.Where(m => m.Author?.Name == username);

    public Message? GetLongestMessage() =>
        Messages.OrderByDescending(m => m.Text.Length).FirstOrDefault();

    public Message? GetShortestMessage() =>
        Messages.OrderBy(m => m.Text.Length).LastOrDefault();

    public IEnumerable<Message> GetMessagesSortedByHex() =>
        Messages.OrderBy(m =>
            string.Concat(m.Text.Select(c => ((int)c).ToString("X2"))));
}