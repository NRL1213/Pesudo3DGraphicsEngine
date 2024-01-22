using System.Numerics;

namespace window_core
{

    static class RenderQueue
    {
        public static Queue<RenderChunk> Rendering = new Queue<RenderChunk>();
    }

    struct RenderChunk
    {
        public Vector2 X; // X start and end on Screen
        public Vector2 Y; // Y start and end on Screen
        public SFML.Graphics.Color colordata; // Color from map
    }

    struct VisionCollision
    {
        public int X; // X on the Map
        public int Y; // Y on the Map
        public float Z; // Distance from player to Collision on the Map
    }
}