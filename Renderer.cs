using System.Data;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using SFML;

namespace EngineMk2
{
    internal class Renderer
    {
        public List<Vector2> FOVtoPoints(float start, float end, int steps, World world)
        {
            float StepSize = (end - start) / steps;
            List<Vector2> FinalList = new List<Vector2>();

            for (float i = start; i <= end; i += StepSize)
            {
                FinalList.Add(RayAtAngle(i, world));
            }

            return FinalList;
        }

        public Vector2 RayAtAngle(float angle, World world) 
        {
            angle = (float)Math.Round(angle);
            angle = angle * (float)Math.PI / 180;
            double Slope = -1 * Math.Tan(angle);

            List<Vector2> LatticePoints = new List<Vector2>();
            for (int X = 0; X < 35; ++X)
            {
                Vector2 Point = new Vector2(X, (float)Math.Floor((X * Math.Tan(angle))));
                LatticePoints.Add(Point);
            }
            for (int Y = 0; Y < 35; ++Y)
            {
                Vector2 Point = new Vector2((float)Math.Floor(1 / Math.Tan(angle) * Y), Y);
                LatticePoints.Add(Point);
            }

            LatticePoints = LatticePoints.OrderBy(X => X.X).Distinct().ToList(); 
            LatticePoints = RemoveOutOfBounds(LatticePoints);
            LatticePoints = RemoveOutOfFov(LatticePoints, angle, world);

            if (angle == Math.PI / 2)
            {
                Console.WriteLine("90 Fixed");
                LatticePoints = new List<Vector2>();
                LatticePoints.Add(new Vector2((float)world.XMax, (float)world.YMax));
            }
            else if (angle == Math.PI)
            {
                Console.WriteLine("180 Fixed");
                LatticePoints = new List<Vector2>();
                LatticePoints.Add(new Vector2((float)world.XMax, (float)world.YMid));
            }
            else if (angle == 3*Math.PI / 2)
            {
                Console.WriteLine("270 Fixed");
                LatticePoints = new List<Vector2>();
                LatticePoints.Add(new Vector2((float)world.XMid, 0));
            }

            foreach (var Point in LatticePoints)
            {
                if (world.Map[(int)Point.X,(int)Point.Y] != 0)
                {
                    Console.WriteLine("Line at " + (angle * (180 / Math.PI)) + " degrees hits a point at " + Point);
                    return Point;
                }
            }
            return LatticePoints.Last();
        }

        public List<Vector2> RemoveOutOfBounds(List<Vector2> LatticePoints) 
        {
            for (int i = 0; i != LatticePoints.Count; ++i)
            {
                LatticePoints[i] = Vector2.Clamp(LatticePoints[i], new Vector2(0,0), new Vector2(34,34));
            }
            return LatticePoints;
        }

        public List<Vector2> RemoveOutOfFov(List<Vector2> LatticePoints, double angle, World map)
        {
            List<Vector2> result = new List<Vector2>();
            bool behind = false;
            bool up = true;
            angle = angle * 180 / Math.PI;
            if (angle > 90 && angle < 180)
                behind = true;
            if (angle > 180 && angle < 360)
                up = false;

            if (angle > 46)
                Console.WriteLine("Stop");

            result = LatticePoints;
            result.RemoveAll(Point => up && Point.Y > (int)map.Player.Y);
            result.RemoveAll(Point => !up && Point.Y < (int)map.Player.Y);
            result.RemoveAll(Point => !behind && Point.X < (int)map.Player.X);
            result.RemoveAll(Point => behind && Point.X > (int)map.Player.X);

            if (result.Count == 0)
                throw new Exception("Bad Point List");

            return result;
        }

        public void FinalRender(List<Vector2> chunks, World world)
        {
            int ChunkNumber = chunks.Count;
            int ChunkWidth = (int)Math.Round((double)Constants.WinLength / ChunkNumber);
            int ChunkHeight = (int)Constants.WinHeight / 2;
            int ChunkCounter = 0;

            foreach (var Chunk in chunks)
            {
                int PlayerX = (int)world.Player.X;
                int PlayerY = (int)world.Player.Y;
                

                int Distance = Math.Max((int)Math.Abs(Chunk.X - PlayerX),(int)Math.Abs(Chunk.Y - PlayerY));
                double TrueDistance = Vector2.Distance(Chunk, world.Player);
                int Middle = (int)Constants.WinHeight / 2; // 540
                int YStart = (int)(Middle + ((Middle / 2) * 1 / TrueDistance));
                int YEnd = (int)(Middle - ((Middle / 2) * 1 / TrueDistance));
                int XStart = ChunkCounter * ChunkWidth;
                int XEnd = (ChunkCounter + 1) * ChunkWidth;
                ChunkCounter++;

                RenderQueue.Enqueue(new RenderChunk
                {
                    X = new Vector2(XStart, XEnd),
                    Y = new Vector2(YStart, YEnd),
                    colordata = GetColor(world.Map[(int)Chunk.X, (int)Chunk.Y]) //SFML.Graphics.Color.White
                }
                );
            }
        }

        public SFML.Graphics.Color GetColor(int colorcode)
        {
            if (colorcode == 1)
                return SFML.Graphics.Color.White;
            if (colorcode == 2)
                return SFML.Graphics.Color.Black;
            if (colorcode == 3)
                return SFML.Graphics.Color.Red;
            if (colorcode == 4)
                return SFML.Graphics.Color.Blue;
            if (colorcode == 5)
                return SFML.Graphics.Color.Yellow;
            if (colorcode == 6)
                return SFML.Graphics.Color.Green;
            else
                return SFML.Graphics.Color.Magenta;
        }

        public void RenderBackground()
        {
            // Floor
            RenderQueue.Enqueue
            (new RenderChunk
            {
                X = new Vector2(0, Constants.WinLength),
                Y = new Vector2(0, Constants.WinHeight / 2),
                colordata = new SFML.Graphics.Color(133, 133, 133)
            }
            );

            // Ceiling
            RenderQueue.Enqueue
            (new RenderChunk
            {
                X = new Vector2(0, Constants.WinLength),
                Y = new Vector2(Constants.WinHeight / 2, Constants.WinHeight),
                colordata = new SFML.Graphics.Color(239, 208, 181)
            }
            );
        }

        public struct RenderChunk
        {
            public Vector2 X;
            public Vector2 Y;
            public SFML.Graphics.Color colordata;
        }

        public static Queue<RenderChunk> RenderQueue = new Queue<RenderChunk>();

    }
}
