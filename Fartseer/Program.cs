using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fartseer
{
	class Program
	{
		static void Main(string[] args)
		{
			Game game = new Game(new SFML.Window.VideoMode(800, 600), "Fartseer!");

			#if DEBUG
				Console.WriteLine("DEBUG BUILD");
				game.ToggleDebug();
			#endif

			if (!game.DoInit(0))
			{
				Console.WriteLine("Game failed to initialize");
				Console.WriteLine("Press any key to stop");
				Console.ReadKey();
				return;
			}
			game.Start();
		}
	}
}
