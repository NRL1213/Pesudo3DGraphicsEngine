using window_core;

namespace LimGEMk1
{
    internal static class InputHandler
    {
        public static void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            var window = (SFML.Window.Window)sender;

            switch(e.Code)
            {
                case SFML.Window.Keyboard.Key.Escape:
                    window.Close();
                    break;
                case SFML.Window.Keyboard.Key.Q:
                    MapData.Direction = (--MapData.Direction + 4) % 4;
                    DebugDirection();
                    break;
                case SFML.Window.Keyboard.Key.E:
                    MapData.Direction = ++MapData.Direction % 4;
                    DebugDirection();
                    break;
                case SFML.Window.Keyboard.Key.W:
                    if (MapData.Player.X != 0)
                        --MapData.Player.X;
                    break;
                case SFML.Window.Keyboard.Key.A:
                    if (MapData.Player.Y != 0)
                        --MapData.Player.Y;
                    break;
                case SFML.Window.Keyboard.Key.S:
                    if (MapData.Player.X != 64)
                        ++MapData.Player.X;
                    break;
                case SFML.Window.Keyboard.Key.D:
                    if (MapData.Player.Y != 64)
                        ++MapData.Player.Y;
                    break;
            }
        }

        private static void DebugDirection()
        {
            switch (MapData.Direction)
            {
                case 0:
                    Console.WriteLine("You are now facing north");
                    break;
                case 1:
                    Console.WriteLine("You are now facing east");
                    break;
                case 2:
                    Console.WriteLine("You are now facing south");
                    break;
                case 3:
                    Console.WriteLine("You are now facing west");
                    break;
                default:
                    break;
            }
        }
    }
}
