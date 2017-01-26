using System;

namespace Off_EE
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
		public static OffEE game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main(string[] args)
        {
			Console.WriteLine("If this is your first run of Off EE, please wait for all data to be initialized. Do not have any mods running or they may contaminate your installation of Off EE. If you have already installed Off EE before, please let this install again.");
			
			game = new OffEE();
			game.Run();
			game.LogFile.Flush();
			game.World.Save(@"data\world.txt");
        }
    }
}
