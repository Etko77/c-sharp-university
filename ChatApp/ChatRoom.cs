using System;
using System.Collections.Generic;
using System.Linq;

public class ChatRoom
{
    public string Name { get; set; }
    public List<Contact> Users { get; set; } = new List<Contact>();
    public List<Message> Messages { get; set; } = new List<Message>();

    public ChatRoom(string name) => Name = name ?? "DefaultRoom";

    public void AddUser(Contact user) =>
        Users.Add(user ?? new Contact(null));

    public void AddMessage(Message message) =>
        Messages.Add(message ?? new Message(null, null));

    public void PrintMessages() =>
        Messages.ForEach(m => Console.WriteLine(m));

    public IEnumerable<Message> GetMessagesByUser(string username) =>
        Messages.Where(m => m.Author?.Name == username);
}