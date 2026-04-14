using System.Collections.Generic;
using System.Linq;

public class ChatRoom
{
    public string Name { get; set; }
    public List<Contact> Users { get; set; } = new();
    public List<Message> Messages { get; set; } = new();

    public ChatRoom(string name) =>
        Name = name ?? "DefaultRoom";

    public void AddUser(Contact user) =>
        Users.Add(user ?? new Contact(null));

    public void AddMessage(Message msg) =>
        Messages.Add(msg ?? new Message(null, null));

    public IEnumerable<Message> GetMessagesByUser(string username) =>
        Messages.Where(m => m.Author?.Name == username);
}