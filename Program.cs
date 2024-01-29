using LimGEMk1;

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
            var MapInstance = new MapData();

            window.KeyPressed += InputHandler.Window_KeyPressed;

            while (window.IsOpen)
            {
                window.DispatchEvents();
                Render(MapInstance);

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

        public void Render(MapData Map)
        {
            List<VisionCollision> collisions = Projecter.ConeProjection(Map, MapData.Direction);
            Renderer.WallRenderer2(collisions, Map);
        }
    }
}
