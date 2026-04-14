using System;

public class Message
{
    public Contact Author { get; set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; } = DateTime.Now;
    public bool IsEdited { get; private set; }

    public Message(Contact? author, string? text)
    {
        Author = author ?? new Contact(null);
        Text = string.IsNullOrWhiteSpace(text) ? "Empty message" : text;
    }

    public void Edit(string? newText) =>
        (Text, IsEdited) = (string.IsNullOrWhiteSpace(newText) ? Text : newText, true);

    public override string ToString() =>
        $"[{CreatedAt:HH:mm:ss}] {Author?.Name}: {Text}" +
        $"{(IsEdited ? " (edited)" : "")}";

    // Deconstruct
    public void Deconstruct(out string author, out string text)
    {
        author = Author?.Name ?? "Unknown";
        text = Text;
    }

    public void Deconstruct(out string author, out string text, out DateTime date)
    {
        author = Author?.Name ?? "Unknown";
        text = Text;
        date = CreatedAt;
    }

    public void Deconstruct(out DateOnly date, out TimeOnly time)
    {
        date = DateOnly.FromDateTime(CreatedAt);
        time = TimeOnly.FromDateTime(CreatedAt);
    }
}