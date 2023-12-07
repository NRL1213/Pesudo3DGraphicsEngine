using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;

static class Constants
{
    public static uint WinLength = 1920;
    public static uint WinHeight = 1080;
}

struct Cord
{
    public double X,Y,Z;
}

struct Face
{
    public Cord Start,End;
}

class Cube
{
    public Cord[] Points = new Cord[8];
    public Cube()
    {
        Points[0] = new Cord { X = 0, Y = 0, Z = 2 };
        Points[1] = new Cord { X = 0, Y = 1, Z = 2 };
        Points[2] = new Cord { X = 1, Y = 0, Z = 2 };
        Points[3] = new Cord { X = 1, Y = 1, Z = 2 };
        Points[4] = new Cord { X = 0, Y = 0, Z = 3 };
        Points[5] = new Cord { X = 0, Y = 1, Z = 3 };
        Points[6] = new Cord { X = 1, Y = 0, Z = 3 };
        Points[7] = new Cord { X = 1, Y = 1, Z = 3 };
    }

    public Face[] Faces = new Face[6];
}

class ViewFrame
{
    public Cord LeftPoint = new() { X = -50, Y = -50, Z = 0 };
    public Cord RightPoint = new() { X = 50, Y = 50, Z = 100 };
}

static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Press ESC key to close window");
        SimpleWindow.Run();
    }
}

static class SimpleWindow
{
    public static void Run()
    {
        var mode = new SFML.Window.VideoMode(Constants.WinLength, Constants.WinHeight);
        var window = new SFML.Graphics.RenderWindow(mode, "Engine Mk1");
        window.SetFramerateLimit(60);
        Cube cube = new();
        Renderer.Get_Cube_Faces(cube);

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Display();
            window.Clear();
        }
    }
}



static class Renderer
{
    public static void Get_Cube_Faces(Cube cube)
    {
        Console.WriteLine("Found all X points that are the same");
    }


}