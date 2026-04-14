using System;

public class Contact
{
    private string name;

    public string Name
    {
        get => name;
        set => name = value ?? GenerateRandomName();
    }

    public Contact(string name) => Name = name;

    private string GenerateRandomName() =>
        $"user{new Random().Next(10000, 99999)}";

    public override string ToString() => Name;
}