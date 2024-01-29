using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using window_core;

namespace LimGEMk1
{
    internal static class Projecter
    {
        public static List<VisionCollision> ConeProjection(MapData Map, int Direction)
        {
            List<VisionCollision> ret = new List<VisionCollision>();
            int Depth = 0;
            int mod = 0;
            int Side = 0;

            switch (Direction)
            {
                case 0:
                    Depth = (int)MapData.Player.X - 1;
                    mod = -1;
                    Side = (int)MapData.Player.Y;
                    break;
                case 1:
                    Depth = (int)MapData.Player.Y + 1;
                    mod = 1;
                    Side = (int)MapData.Player.X;
                    break;
                case 2:
                    Depth = (int)MapData.Player.X + 1;
                    mod = 1;
                    Side = (int)MapData.Player.Y;
                    break;
                case 3:
                    Depth = (int)MapData.Player.Y - 1;
                    mod = -1;
                    Side = (int)MapData.Player.X;
                    break;
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
    }
}
