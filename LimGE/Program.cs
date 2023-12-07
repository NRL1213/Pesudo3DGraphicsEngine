using SFML.Graphics;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
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
    public Cord P1,P2,P3,P4;
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

static class ViewFrame
{
    public static Cord LeftPoint = new() { X = -960, Y = -540, Z = 0 };
    public static Cord RightPoint = new() { X = 960, Y = 540, Z = 100 };
}

static class Frame
{
    public static List<Drawable> Draws = new List<Drawable>();
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
        Renderer.World_To_Screen_Space(new Cord{X = 0, Y = 0, Z  = 0}, cube.Faces[0]);

        while (window.IsOpen)
        {
            foreach(var Drawing in Frame.Draws)
            {
                window.Draw(Drawing);
            }
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
        List<Cord> points = cube.Points.GroupBy(p => p.Z)
            .Where(group => group.Select(p => p.X).Distinct().Count() > 1 || group.Select(p => p.Y).Distinct().Count() > 1)
            .SelectMany(group => group)
            .ToList();

        points = points.Concat(cube.Points.GroupBy(p => p.Y)
            .Where(group => group.Select(p => p.X).Distinct().Count() > 1 || group.Select(p => p.Z).Distinct().Count() > 1)
            .SelectMany(group => group).ToList())
            .ToList();

        points = points.Concat(cube.Points.GroupBy(p => p.X)
            .Where(group => group.Select(p => p.Y).Distinct().Count() > 1 || group.Select(p => p.Z).Distinct().Count() > 1)
            .SelectMany(group => group).ToList())
            .ToList();


        for (int i = 0; i != 6; i++)
        {
            cube.Faces[i] = new Face { P1 = points[i * 4], P2 = points[(i * 4) + 1], P3 = points[(i * 4) + 2], P4 = points[(i * 4) + 3] };
        }
    }

    public static Cord Get_Face_Center_Point(Face face)
    {
        return new Cord
        {
            X = (face.P1.X + face.P2.X + face.P3.X + face.P4.X) / 4,
            Y = (face.P1.Y + face.P2.Y + face.P3.Y + face.P4.Y) / 4,
            Z = (face.P1.Z + face.P2.Z + face.P3.Z + face.P4.Z) / 4,
        };
    }

    public static void Perspective_Shift(Cord Player, Cord point)
    {
            double ZAngle = Math.Atan2(point.X - Player.X, point.Y - Player.Y); // X-Y slope
            double YAngle = Math.Atan2(point.Z - Player.Z, point.X - Player.X); // X-Z slope
            double XAngle = Math.Atan2(point.Z - Player.Z, point.Y - Player.Y); // Y-Z Slope

            //X is left Right
            //Y is Forward Back
            //Z is height
            double XYAngle = ZAngle * 180 / Math.PI;
            double YZAngle = XAngle * 180 / Math.PI;
            double XZAngle = YAngle * 180 / Math.PI;

            // Work on a way to take the viewing angle and distort thee points using that
    }

    public static Vector2 World_To_Screen_Space(Cord Camera_Pos, Face face)
    {
        //Get Distortion of points from play point of view (Angles gotten in Perspective shift)
        //Scale with distance
        //Center on screen with 0,0 being 0,0,0
        //Build shape and display

        Face face2 = face;
        Cord Center = Get_Face_Center_Point(face);
        double Dist = Math.Sqrt(Math.Pow(Center.X - Camera_Pos.X, 2) + Math.Pow(Center.Y - Camera_Pos.Y, 2) + Math.Pow(Center.Z - Camera_Pos.Z, 2));
        face2 = Scale_Face(face, Dist, Center);
        Shape_Builder(face2, 100);
        return new Vector2();
    }

    public static Face Scale_Face(Face face, double Distance, Cord Center)
    {
        Distance = 1 / Distance;
        face.P1 = new Cord { X = face.P1.X * Center.X * Distance, Y = face.P1.Y * Center.Y * Distance, Z = face.P1.Z};
        face.P2 = new Cord { X = face.P2.X * Center.X * Distance, Y = face.P2.Y * Center.Y * Distance, Z = face.P2.Z};
        face.P3 = new Cord { X = face.P3.X * Center.X * Distance, Y = face.P3.Y * Center.Y * Distance, Z = face.P3.Z};
        face.P4 = new Cord { X = face.P4.X * Center.X * Distance, Y = face.P4.Y * Center.Y * Distance, Z = face.P4.Z};

        return face;
    }

    public static void Shape_Builder(Face face, double scale)
    {
        double XSize = 0;
        double YSize = 0;
        int XStart = 0;
        int YStart = 0;

        List<Cord> FaceCords = new List<Cord>{face.P1, face.P2, face.P3, face.P4 };

        XSize = FaceCords.Max(Point => Point.X) - FaceCords.Min(Point => Point.X);
        YSize = FaceCords.Max(Point => Point.Y) - FaceCords.Min(Point => Point.Y);
        XSize = XSize * scale;
        YSize = YSize * scale;
        XStart = (int)Math.Round(FaceCords.Min(Point => Point.X));
        YStart = (int)Math.Round(FaceCords.Min(Point => Point.Y));
    }

    public static void Shape_Finilizer(double XSize, double YSize, int XStart, int YStart)
    {
        SFML.Graphics.RectangleShape Shape = new SFML.Graphics.RectangleShape(new SFML.System.Vector2f((float)XSize, (float)YSize));
        XStart = XStart + (int)Constants.WinLength / 2;
        YStart = YStart + (int)(Constants.WinHeight / 2);
        Shape.Position = new SFML.System.Vector2f(XStart, YStart);
        Frame.Draws.Add(Shape);
    }
}