using System;

namespace TestGame
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new RPGGame())
                game.Run();
        }
    }
}
