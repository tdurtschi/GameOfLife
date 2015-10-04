using System;
using System.Text;

namespace GameOfLife
{
	internal class Program
	{
		public const int DefaultWidth = 80;

		public const int DefaultHeight = 40;

		public const int DefaultSpeed = 300;

		public static char AliveChar;

		public static char DeadChar;

		public static GameOfLife game;

		private static void Main( string[] args )
		{
			try
			{
				AliveChar = Encoding.GetEncoding( 437 ).GetChars( new byte[] { 4 } )[0];
				DeadChar = Encoding.GetEncoding( 437 ).GetChars( new byte[] { 0 } )[0];

				bool isInteractive = false;
				bool argsValid = false;
				int consoleWidth = 0,
					consoleHeight = 0,
					speed = 0;

				if( args.Length < 1 )
				{
					isInteractive = argsValid = true;
				}
				else if( args.Length == 1 && int.TryParse( args[0], out speed ) )
				{
					argsValid = true;
					consoleHeight = DefaultHeight;
					consoleWidth = DefaultWidth;
				}
				else if( args.Length == 3 && int.TryParse( args[0], out speed )
						&& int.TryParse( args[1], out consoleWidth )
						&& int.TryParse( args[2], out consoleHeight ) )
				{
					argsValid = true;
				}

				if( isInteractive )
				{
					Console.WriteLine( "Running interactively." );
					Console.Write( "Set speed (in milliseconds): " );
					if( int.TryParse( Console.ReadLine(), out speed ) )
					{
						if( speed < 50 || speed > 10000 )
						{
							Console.WriteLine( "speed out of range, using default value: {0}", DefaultSpeed );
							speed = DefaultSpeed;
						}
					}
					else
					{
						speed = DefaultSpeed;
					}
					Console.Write( "Set console width: " );
					if( int.TryParse( Console.ReadLine(), out consoleWidth ) )
					{
						if( consoleWidth < 10 || consoleWidth > 150 )
						{
							Console.WriteLine( "width out of range, using default value: {0}", DefaultWidth );
							consoleWidth = DefaultWidth;
						}
					}
					else
					{
						consoleWidth = DefaultWidth;
					}
					Console.Write( "Set console height: " );
					if( int.TryParse( Console.ReadLine(), out consoleHeight ) )
					{
						if( consoleHeight < 5 || consoleHeight > 80 )
						{
							Console.WriteLine( "height out of range, using default value: {0}", DefaultHeight );
							consoleWidth = DefaultHeight;
						}
					}
					else
					{
						consoleHeight = DefaultHeight;
					}
				}

				if( !argsValid )
				{
					Console.WriteLine( "Usage: gameoflife.exe [speed [console-width console-height]]\n"
										+ "Where\n"
										+ "\tspeed: refresh rate (in ms)\n"
										+ "\tconsolewidth: width in characters (20-150)\n"
										+ "\tconsoleheight: height in characters (10-80)\n" );

					return;
				}

				game = new GameOfLife( consoleWidth, consoleHeight, speed, AliveChar, DeadChar );
				game.DoSetup();
				game.DoLoop();
			}
			catch( Exception ex )
			{
				Console.Clear();
				Console.WriteLine( "ERROR:" );
				Console.WriteLine( ex.Message );
			}
		}

	}
}
