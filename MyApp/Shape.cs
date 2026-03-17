using System;

interface IElliptical
{
    bool isElliptical();
}

abstract class Shape
{
    private int color; 

    public class BasicColors
    {
        public static int Blue = 1;
        public static int Red = 2;
        public static int Green = 3;
    }

    public int Color
    {
        get
        {
            int r = (color >> 16) & 0xFF;
            int g = (color >> 8) & 0xFF;
            int b = color & 0xFF;

            if (r == 0 && g == 0 && b == 255)
                return BasicColors.Blue;
            else if (r == 255 && g == 0 && b == 0)
                return BasicColors.Red;
            else if (r == 0 && g == 255 && b == 0)
                return BasicColors.Green;

            return 0; 
        }
        set
        {
            int alpha = 255; 

            switch (value)
            {
                case 1: 
                    color = (alpha << 24) | (0 << 16) | (0 << 8) | 255;
                    break;

                case 2: 
                    color = (alpha << 24) | (255 << 16) | (0 << 8) | 0;
                    break;

                case 3: 
                    color = (alpha << 24) | (0 << 16) | (255 << 8) | 0;
                    break;

                default:
                    throw new ArgumentException("Невалиден цвят!");
            }
        }
    }

    public abstract double Perimeter();
    public abstract double Area();
}

class Rectangle: Shape, IElliptical
{
        protected double width;
        protected double height;
    public Rectangle(double width, double height)
    {
        this.width = width;
        this.height = height;
    }
    public override double Area()
    {
        return width * height;
    }
    public override double Perimeter()
    {
        return 2 * ( width + height);
    }
    public bool isElliptical()
    {
        return false;
    }
}
class Circle : Shape, IElliptical
{
    private double radius;

    public Circle(double radius)
    {
        this.radius = radius;
    }

    public override double Perimeter()
    {
        return 2 * Math.PI * radius;
    }

    public override double Area()
    {
        return Math.PI * radius * radius;
    }

    public bool isElliptical()
    {
        return true;
    }
}

class Square : Rectangle
{
    public Square(double side) : base(side, side)
    {
    }

    public static double CalculateArea(double side)
    {
        return side * side;
    }
}

class Triangle<T> where T : struct
{
    public T A { get; private set; }
    public T B { get; private set; }
    public T C { get; private set; }

    private Triangle(T a, T b, T c)
    {
        A = a;
        B = b;
        C = c;
    }

    public static bool GetInstance(T a, T b, T c, out Triangle<T>? triangle)
    {
        triangle = null;

        if (typeof(T) != typeof(int) && typeof(T) != typeof(float))
            return false;

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        double dc = Convert.ToDouble(c);

        if (da + db > dc && da + dc > db && db + dc > da)
        {
            triangle = new Triangle<T>(a, b, c);
            return true;
        }

        return false;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== CREATE SHAPE ===");
        Console.WriteLine("1 - Rectangle");
        Console.WriteLine("2 - Circle");
        Console.WriteLine("3 - Square");
        Console.Write("Choose shape: ");

        int choice = int.Parse(Console.ReadLine()!);

        Shape? shape = null;

        switch (choice)
        {
            case 1:
                Console.Write("Width: ");
                double w = double.Parse(Console.ReadLine()!);
                Console.Write("Height: ");
                double h = double.Parse(Console.ReadLine()!);
                shape = new Rectangle(w, h);
                break;

            case 2:
                Console.Write("Radius: ");
                double r = double.Parse(Console.ReadLine()!);
                shape = new Circle(r);
                break;

            case 3:
                Console.Write("Side: ");
                double s = double.Parse(Console.ReadLine()!);
                shape = new Square(s);
                break;

            default:
                Console.WriteLine("Invalid choice!");
                return;
        }

        Console.WriteLine("\nChoose color:");
        Console.WriteLine("1 - Blue");
        Console.WriteLine("2 - Red");
        Console.WriteLine("3 - Green");
        Console.Write("Color: ");

        int colorChoice = int.Parse(Console.ReadLine()!);
        shape.Color = colorChoice;

        Console.WriteLine("\n=== RESULT ===");
        Console.WriteLine($"Area: {shape.Area():F2}");
        Console.WriteLine($"Perimeter: {shape.Perimeter():F2}");
        Console.WriteLine($"Color (1-Blue,2-Red,3-Green): {shape.Color}");

        // -------- TRIANGLE --------

        Console.WriteLine("\n=== CREATE TRIANGLE ===");
        Console.WriteLine("1 - int");
        Console.WriteLine("2 - float");
        Console.Write("Choose type: ");

        int typeChoice = int.Parse(Console.ReadLine()!);

        if (typeChoice == 1)
        {
            Console.Write("Side A: ");
            int a = int.Parse(Console.ReadLine()!);
            Console.Write("Side B: ");
            int b = int.Parse(Console.ReadLine()!);
            Console.Write("Side C: ");
            int c = int.Parse(Console.ReadLine()!);

            if (Triangle<int>.GetInstance(a, b, c, out Triangle<int>? t))
                Console.WriteLine("Triangle created successfully!");
            else
                Console.WriteLine("Invalid triangle!");
        }
        else if (typeChoice == 2)
        {
            Console.Write("Side A: ");
            float a = float.Parse(Console.ReadLine()!);
            Console.Write("Side B: ");
            float b = float.Parse(Console.ReadLine()!);
            Console.Write("Side C: ");
            float c = float.Parse(Console.ReadLine()!);

            if (Triangle<float>.GetInstance(a, b, c, out Triangle<float>? t))
                Console.WriteLine("Triangle created successfully!");
            else
                Console.WriteLine("Invalid triangle!");
        }
        else
        {
            Console.WriteLine("Invalid type!");
        }
    }
}
