using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using window_core;

namespace LimGEMk1
{
    internal static class Renderer
    {
        public static void RenderBackground()
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

        public static void WallRenderer2(List<VisionCollision> WallList, MapData Map)
        {
            Renderer.RenderBackground();

            int Chunks = WallList.Count;
            for (int i = 0; i != WallList.Count(); ++i)
            {
                var Collision = WallList[i];
                var Color = Renderer.ColorPicker(Map.Map[Collision.Y, Collision.X]);
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

        public static SFML.Graphics.Color ColorPicker(int MapNum)
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
    }
}
