using System.Diagnostics.Contracts;
using System.Numerics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace window_core
{
    static class Constants
    {
        public static uint WinLength = 1920;
        public static uint WinHeight = 1080;
    }

    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press ESC key to close window");
            var window = new SimpleWindow();
            window.Run();
        }
    }

    class SimpleWindow
    {
        public void Run()
        {
            var mode = new SFML.Window.VideoMode(Constants.WinLength, Constants.WinHeight);
            var window = new SFML.Graphics.RenderWindow(mode, "Engine Mk1");
            window.SetFramerateLimit(60);
            var FinalRender = new Renderer();
            var MapInstance = new MapData();

            window.KeyPressed += Window_KeyPressed;

            while (window.IsOpen)
            {
                window.DispatchEvents();
                FinalRender.Render(MapInstance);

                foreach (var RenderingChunk in RenderQueue.Rendering)
                {
                    var Drawing = new SFML.Graphics.RectangleShape(new SFML.System.Vector2f(RenderingChunk.X.Y - RenderingChunk.X.X, RenderingChunk.Y.Y - RenderingChunk.Y.X));
                    Drawing.Position = new SFML.System.Vector2f(RenderingChunk.X.X, RenderingChunk.Y.X);
                    Drawing.FillColor = RenderingChunk.colordata;
                    window.Draw(Drawing);
                }
                RenderQueue.Rendering.Clear();

                window.Display();
                window.Clear();
            }
        }

        private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            var window = (SFML.Window.Window)sender;
            if (e.Code == SFML.Window.Keyboard.Key.Escape)
            {
                window.Close();
            }
            if (e.Code == SFML.Window.Keyboard.Key.Q)
            {
                MapData.Direction = (--MapData.Direction + 4) % 4;
                if (MapData.Direction == 0)
                    Console.WriteLine("You are now facing north");
                if (MapData.Direction == 1)
                    Console.WriteLine("You are now facing east");
                if (MapData.Direction == 2)
                    Console.WriteLine("You are now facing south");
                if (MapData.Direction == 3)
                    Console.WriteLine("You are now facing west");

            }
            if (e.Code == SFML.Window.Keyboard.Key.E)
            {
                MapData.Direction = ++MapData.Direction % 4;
                if (MapData.Direction == 0)
                    Console.WriteLine("You are now facing north");
                if (MapData.Direction == 1)
                    Console.WriteLine("You are now facing east");
                if (MapData.Direction == 2)
                    Console.WriteLine("You are now facing south");
                if (MapData.Direction == 3)
                    Console.WriteLine("You are now facing west");
            }
            if (e.Code == SFML.Window.Keyboard.Key.W)
            {
                if (MapData.Player.X != 0)
                    --MapData.Player.X;
            }
            if (e.Code == SFML.Window.Keyboard.Key.D)
            {
                if (MapData.Player.Y != 64)
                    ++MapData.Player.Y;
            }
            if (e.Code == SFML.Window.Keyboard.Key.S)
            {
                if (MapData.Player.X != 64)
                    ++MapData.Player.X;
            }
            if (e.Code == SFML.Window.Keyboard.Key.A)
            {
                if (MapData.Player.Y != 0)
                    --MapData.Player.Y;
            }
        }
    }

    class Renderer
    {
        public void Render(MapData Map)
        {
            List<VisionCollision> collisions = ConeProjection(Map, MapData.Direction);
            WallRenderer2(collisions, Map);
        }

        public List<VisionCollision> ConeProjection(MapData Map, int Direction)
        {
            List<VisionCollision> ret = new List<VisionCollision>();
            int Depth = 0;
            int mod = 0;
            int Side = 0;

            if (Direction == 0)
            {
                Depth = (int)MapData.Player.X - 1;
                mod = -1;
                Side = (int)MapData.Player.Y;
            }
            else if (Direction == 1) 
            {
                Depth = (int)MapData.Player.Y + 1;
                mod = 1;
                Side = (int)MapData.Player.X;
            }
            else if (Direction == 2)
            {
                Depth = (int)MapData.Player.X + 1;
                mod = 1;
                Side = (int)MapData.Player.Y;
            }
            else if (Direction == 3)
            {
                Depth = (int)MapData.Player.Y - 1;
                mod = -1;
                Side = (int)MapData.Player.X;
            }

            int LeftConeOffset = -1;
            int RightConeOffset = 1;
            bool LeftDone = false;
            bool RightDone = false;
            int MapNum = 0;

            while (Depth <= 64 && Depth >= 0)
            {
                for (int i = LeftConeOffset; i <= RightConeOffset; ++i)
                {
                    if (Direction == 0 || Direction == 2)
                        MapNum = Map.Map[Depth, Side + i];
                    else if (Direction == 1 || Direction == 3)
                        MapNum = Map.Map[Side + i, Depth];
                    if (Side + i <= 64 && Side + i >= 0 && MapNum != 0)
                    {
                        if (Direction == 0 || Direction == 2)
                        {
                            ret.Add(new VisionCollision
                            {
                                X = Side + i,
                                Y = Depth,
                                Z = Math.Abs(Depth - (int)MapData.Player.X),
                            });
                        }
                        else
                        {
                            ret.Add(new VisionCollision
                            {
                                Y = Side + i,
                                X = Depth,
                                Z = Math.Abs(Depth - (int)MapData.Player.Y),
                            });
                        }

                        if (i == LeftConeOffset)
                            LeftDone = true;
                        if (i == RightConeOffset)
                            RightDone = true;
                    }
                }

                if (!LeftDone)
                    --LeftConeOffset;
                if (!RightDone)
                    ++RightConeOffset;
                Depth += mod;
            }

            List<VisionCollision> ret_Sorted = ret;

            if (Direction == 2 || Direction == 0)
            {
                ret_Sorted = ret_Sorted
                .OrderBy(point => point.X)
                .ThenBy(point => point.Y)
                .GroupBy(point => point.X)
                .Select(group => group.OrderBy(point => point.Z).First())
                .ToList();
            }
            else
            {
                ret_Sorted = ret_Sorted
                .OrderBy(point => point.Y)
                .ThenBy(point => point.X)
                .GroupBy(point => point.Y)
                .Select(group => group.OrderBy(point => point.Z).First())
                .ToList();
            }


            return ret_Sorted;
        }

        public void WallRenderer2(List<VisionCollision> WallList, MapData Map)
        {
            RenderBackground();

            int Chunks = WallList.Count;
            for (int i = 0; i != WallList.Count(); ++i)
            {
                var Collision = WallList[i];
                var Color = ColorPicker(Map.Map[Collision.Y, Collision.X]);
                float firstX = i * Constants.WinLength / Chunks;
                float SecondX = (i + 1) * Constants.WinLength / Chunks;
                float Scaler = 1 / Collision.Z;
                float Size = Constants.WinHeight / 2; //540 or half the screen
                float YScale1 = (Size / 2) * Scaler;
                float YScale2 = (YScale1 + (Size / 2)) * Scaler;
                float YStart = (Constants.WinHeight / 2) - YScale1;
                float YEnd = (Constants.WinHeight / 2) + YScale2;
                RenderQueue.Rendering.Enqueue
                (new RenderChunk
                {
                    X = new Vector2(firstX, SecondX),
                    Y = new Vector2(YStart, YEnd),
                    colordata = Color
                }
                );
            }
        }

        public void RenderBackground()
        {
            // Floor
            RenderQueue.Rendering.Enqueue
            (new RenderChunk
            {
                X = new Vector2(0, Constants.WinLength),
                Y = new Vector2(0, Constants.WinHeight / 2),
                colordata = new SFML.Graphics.Color(133, 133, 133)
            }
            );

            // Ceiling
            RenderQueue.Rendering.Enqueue
            (new RenderChunk
            {
                X = new Vector2(0, Constants.WinLength),
                Y = new Vector2(Constants.WinHeight / 2, Constants.WinHeight),
                colordata = new SFML.Graphics.Color(239, 208, 181)
            }
            );
        }

        public SFML.Graphics.Color ColorPicker(int MapNum)
        {
            if (MapNum == 1)
                return SFML.Graphics.Color.Red;
            else if (MapNum == 2)
                return SFML.Graphics.Color.Blue;
            else if (MapNum == 3)
                return SFML.Graphics.Color.Yellow;
            else if (MapNum == 4)
                return SFML.Graphics.Color.Green;
            else if (MapNum == 5)
                return SFML.Graphics.Color.Magenta;
            else
                return SFML.Graphics.Color.White;
        }

        public bool OutOfBounds(int X, int Y, MapData Map)
        {
            return X >= Map.Map.GetLength(1) || X < 0 || Y >= Map.Map.GetLength(0) || Y <= 0;
        }
    }

}