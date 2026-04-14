using System;

public class Message
{
    public Contact Author { get; set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; } = DateTime.Now;
    public bool IsEdited { get; private set; }

    public Message(Contact author, string text)
    {
        Author = author ?? new Contact(null);
        Text = text ?? "Empty message";
    }

    public void Edit(string newText) =>
        (Text, IsEdited) = (newText ?? Text, true);

    public override string ToString() =>
        $"[{CreatedAt:HH:mm:ss}] {Author?.Name}: {Text}" +
        $"{(IsEdited ? " (edited)" : "")}";
}