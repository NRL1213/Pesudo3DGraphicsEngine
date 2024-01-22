using System.Diagnostics.Contracts;
using System.Numerics;
using System.Linq;

namespace window_core
{
    class Renderers
    {
        public List<VisionCollision> GrabAllPoints(MapData Map)
        {
            int Radius = 32; // R * 2 + 1 = View Cone
            int LeftBound = (int)Map.Player.Y - (Radius + 1);
            int RightBound = (int)Map.Player.Y + (Radius - 1);
            int depth = 0;
            List<VisionCollision> WallList = new List<VisionCollision>();

            for (int i = LeftBound; i <= RightBound; ++i)
            {
                if (i != LeftBound || i != RightBound)
                {
                    depth = 0;
                    while (Map.Map[(int)Map.Player.X + depth, i] == 0)
                    {
                        ++depth;
                    }

                    float Distance = Vector2.Distance(Map.Player, new Vector2((int)Map.Player.X + depth, Map.Player.Y));
                    float LineDistance = Vector2.Distance(Map.Player, new Vector2((int)Map.Player.X + depth, i));
                    WallList.Add(new VisionCollision
                    {
                        X = i,
                        Y = (int)Map.Player.X + depth,
                        Z = Distance
                    }
                    );
                }
                else if (i == RightBound)
                {
                    WallList.Add(new VisionCollision
                    {
                        X = 0,
                        Y = 64,
                        Z = 31
                    }
                    );
                }
                else if (i == LeftBound)
                {
                    WallList.Add(new VisionCollision
                    {
                        X = 64,
                        Y = 64,
                        Z = 31
                    }
                    );
                }
            }
            return WallList;
        }

        public VisionCollision CastRays(MapData Map, int angle)
        {
            double Rangle = angle * (Math.PI / 180);
            List<Vector2> Blocks = new List<Vector2>();
            int UpDown = -1;
            int LeftRight = -1;
            if (angle < 90 || angle > 270)
            {
                LeftRight = 1;
            }
            if (angle < 180)
            {
                UpDown = 1;
            }



            for (int i = (int)Map.Player.Y; i < 64 && i > -1; i += LeftRight)
            {
                if ((float)Math.Floor(i / Math.Tan(Rangle)) >= 0 && i > 0)
                {
                    Blocks.Add(new Vector2(i, (float)Math.Floor(i * Math.Tan(Rangle))));
                }
            }
            Blocks = Blocks.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();
            int max = Blocks.Count;


            for (int i = (int)Math.Floor(Blocks[1].Y) + 1; i != max - 1 && i > -1; i += UpDown)
            {
                if (!Blocks.Contains(new Vector2((float)Math.Floor(i / Math.Tan(Rangle)), i)) && (float)Math.Floor(i / Math.Tan(Rangle)) >= 0 && i > 0)
                {
                    Blocks.Add(new Vector2((float)Math.Floor(i / Math.Tan(Rangle)), i));
                }
            }

            Blocks = Blocks.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();

            foreach (var Block in Blocks)
            {
                Console.WriteLine(Block);
                if (Map.Map[(int)Block.X, (int)Block.Y] != 0)
                {
                    return new VisionCollision
                    {
                        X = (int)Block.X,
                        Y = (int)Block.Y,
                        Z = Vector2.Distance(Block, Map.Player)
                    };
                }
                //Console.WriteLine(Block);
            }
            return new VisionCollision();

        }
    }
}