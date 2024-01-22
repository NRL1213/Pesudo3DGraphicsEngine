using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Graphics;

namespace EngineMk2
{
    public static class Constants
    {
        public static uint WinLength = 1920;
        public static uint WinHeight = 1080;
    }

    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press ESC key to close window");

            var Map = new World();
            var Render = new Renderer();
            var window = new SimpleWindow(Map, Render);
            window.Run();
        }
    }

    class SimpleWindow
    {
        public World Map;
        public Renderer Renderer;
        public int FovStart = 1;
        public int FovEnd = 89;

        public SimpleWindow(World map, Renderer renderer)
        {
            Map = map;
            Renderer = renderer;
        }

        public void Run()
        {
            var mode = new SFML.Window.VideoMode(Constants.WinLength, Constants.WinHeight);
            var window = new SFML.Graphics.RenderWindow(mode, "Engine Mk1");
            window.SetFramerateLimit(60);

            window.KeyPressed += Window_KeyPressed;

            while (window.IsOpen)
            {
                Renderer.RenderBackground();
                Renderer.FinalRender(Renderer.FOVtoPoints(FovStart, FovEnd, 88, Map), Map);

                foreach (var Chunk in Renderer.RenderQueue)
                {
                    var DrawChunk = new RectangleShape(new SFML.System.Vector2f(Chunk.X.Y - Chunk.X.X, Chunk.Y.Y - Chunk.Y.X));
                    DrawChunk.Position = new SFML.System.Vector2f(Chunk.X.X, Chunk.Y.X);
                    DrawChunk.FillColor = Chunk.colordata;
                    window.Draw(DrawChunk);
                }
                window.DispatchEvents();
                window.Display();
                window.Clear();
                Renderer.RenderQueue.Clear();
            }
        }

        private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            var window = (SFML.Window.Window)sender;
            if (e.Code == SFML.Window.Keyboard.Key.Escape)
            {
                window.Close();
            }
            Console.WriteLine(FovStart.ToString() + " " + FovEnd.ToString());

        }
    }
}