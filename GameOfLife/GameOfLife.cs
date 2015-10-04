using System;
using System.Threading;

namespace GameOfLife
{
	public class GameOfLife
	{
		public char AliveChar
		{
			get;
			set;
		}

		public char DeadChar
		{
			get;
			set;
		}

		public int ConsoleWidth
		{
			get;
			set;
		}

		public int ConsoleHeight
		{
			get;
			set;
		}

		public int Speed
		{
			get;
			set;
		}

		private bool SwitchGridFlag = true;

		private bool Cancel;

		public char[,] GameGrid;

		public char[,] UpdatedGameGrid;

		public GameOfLife( int width, int height, int speed, char aliveChar, char deadChar )
		{
			ConsoleWidth = width;
			ConsoleHeight = height;
			Speed = speed;

			Console.SetWindowSize( ConsoleWidth, ConsoleHeight );
			Console.CancelKeyPress += HandleCancelKeyPress;

			AliveChar = aliveChar;
			DeadChar = deadChar;

			GameGrid = new char[ConsoleWidth, ConsoleHeight];
			UpdatedGameGrid = new char[ConsoleWidth, ConsoleHeight];

			Console.Clear();
		}

		private void HandleCancelKeyPress( object sender, ConsoleCancelEventArgs e )
		{
			e.Cancel = true;
			Cancel = true;

			Environment.Exit( 0 );
		}

		public void DoSetup()
		{
			var TopGrid = _GetTopGrid();

			_WriteFullGridToConsole();
			ConsoleKeyInfo keypress;

			var xPos = 0;
			var yPos = 0;

			Console.CursorVisible = false;

			do
			{
				Console.SetCursorPosition( xPos, yPos );
				Console.Write( "X" );

				keypress = Console.ReadKey();
				Console.SetCursorPosition( xPos, yPos );

				switch( keypress.Key )
				{
					case ConsoleKey.LeftArrow:
						Console.Write( TopGrid[xPos, yPos] );
						Console.Write( TopGrid[xPos + 1, yPos] );
						xPos--;
						xPos = xPos <= 0 ? 0 : xPos;
						break;
					case ConsoleKey.RightArrow:
						Console.Write( TopGrid[xPos, yPos] );
						xPos++;
						xPos = xPos > ConsoleWidth ? ConsoleWidth : xPos;
						break;
					case ConsoleKey.UpArrow:
						Console.Write( TopGrid[xPos, yPos] );
						Console.Write( TopGrid[xPos + 1, yPos] );
						yPos--;
						yPos = yPos < 0 ? 0 : yPos;
						break;
					case ConsoleKey.DownArrow:
						Console.Write( TopGrid[xPos, yPos] );
						Console.Write( TopGrid[xPos + 1, yPos] );
						yPos++;
						yPos = yPos > ConsoleHeight ? ConsoleHeight : yPos;
						break;
					case ConsoleKey.H:
						_DisplayHelp();
						_WriteFullGridToConsole();
						break;
					case ConsoleKey.S:
						_GenerateSoup();
						break;
					case ConsoleKey.Enter:
						TopGrid[xPos, yPos] = TopGrid[xPos, yPos] == AliveChar ? DeadChar : AliveChar;
						break;
					case ConsoleKey.Delete:
						GameGrid = new char[ConsoleWidth, ConsoleHeight];
						UpdatedGameGrid = new char[ConsoleWidth, ConsoleHeight];
						TopGrid = _GetTopGrid();
						_WriteFullGridToConsole();
						break;
				}
			}
			while( keypress.Key != ConsoleKey.Escape );

			_WriteFullGridToConsole();
		}

		private void _DisplayHelp()
		{
			Console.Clear();
			Console.WriteLine( "Commands:\n" +
				"H - Toggle this help screen\n" +
				"Escape - Toggle between setup and live mode\n"+
				"Up/Down/Left/Right - Position cursor in setup mode\n" +
				"Enter - Toggle the current cell\n"+
				"Delete - Clear grid\n"+
				"S - Generate soup (a random distribution of live and dead cells)\n" +
				"Ctrl + C - exit program");
			Console.WriteLine( "\nPress enter to continue... " );
			Console.ReadLine();
		}

		public void DoLoop()
		{
			while( !Cancel )
			{
				_WriteGridToConsole();
				_UpdateGrid();

				if( Console.KeyAvailable )
				{
					var keyPress = Console.ReadKey();

					if( keyPress.Key == ConsoleKey.Escape || keyPress.Key == ConsoleKey.Q )
					{
						DoSetup();
					}
				}

				Thread.Sleep( Speed );
			}
		}

		private void _UpdateGrid()
		{
			char[,] currentGrid = _GetTopGrid();
			char[,] newGrid = _GetBottomGrid();

			for( var indexY = 0; indexY < ConsoleHeight; indexY++ )
			{
				for( var indexX = 0; indexX < ConsoleWidth; indexX++ )
				{
					var alive = currentGrid[indexX, indexY] == AliveChar;

					var liveCells = 0;

					if( indexX < ConsoleWidth - 1 && currentGrid[indexX + 1, indexY] == AliveChar )
					{
						liveCells++;
					}
					if( indexX > 0 && currentGrid[indexX - 1, indexY] == AliveChar )
					{
						liveCells++;
					}
					if( indexX < ConsoleWidth - 1 && indexY > 0 && currentGrid[indexX + 1, indexY - 1] == AliveChar )
					{
						liveCells++;
					}
					if( indexY > 0 && currentGrid[indexX, indexY - 1] == AliveChar )
					{
						liveCells++;
					}
					if( indexX > 0 && indexY > 0 && currentGrid[indexX - 1, indexY - 1] == AliveChar )
					{
						liveCells++;
					}
					if( indexX < ConsoleWidth - 1 && indexY < ConsoleHeight - 1 && currentGrid[indexX + 1, indexY + 1] == AliveChar )
					{
						liveCells++;
					}
					if( indexY < ConsoleHeight - 1 && currentGrid[indexX, indexY + 1] == AliveChar )
					{
						liveCells++;
					}
					if( indexX > 0 && indexY < ConsoleHeight - 1 && currentGrid[indexX - 1, indexY + 1] == AliveChar )
					{
						liveCells++;
					}

					if( alive )
					{
						if( liveCells < 2 )
						{
							newGrid[indexX, indexY] = DeadChar;
						}
						else if( liveCells > 3 )
						{
							newGrid[indexX, indexY] = DeadChar;
						}
						else
						{
							newGrid[indexX, indexY] = AliveChar;
						}
					}
					else if( liveCells == 3 )
					{
						newGrid[indexX, indexY] = AliveChar;
					}
					else
					{
						newGrid[indexX, indexY] = DeadChar;
					}
				}
			}

			SwitchGridFlag = !SwitchGridFlag;
		}

		private char[,] _GetTopGrid()
		{
			return SwitchGridFlag ? GameGrid : UpdatedGameGrid;
		}


		private char[,] _GetBottomGrid()
		{
			return SwitchGridFlag ? UpdatedGameGrid : GameGrid;
		}

		private void _GenerateSoup()
		{
			var topGrid = _GetTopGrid();
			var generator = new Random();

			for( int indexY = 5; indexY < ConsoleHeight - 5; indexY++ )
			{
				for( int indexX = 5; indexX < ConsoleWidth - 5; indexX++ )
				{
					topGrid[indexX, indexY] = generator.Next( 0, 12 ) == 1 ? AliveChar : DeadChar;
				}
			}

			_WriteFullGridToConsole();
		}


		private void _WriteFullGridToConsole()
		{
			Console.Clear();

			char[,] currentGrid = _GetTopGrid();

			Console.SetCursorPosition( 0, 0 );
			for( int indexY = 0; indexY < ConsoleHeight; indexY++ )
			{
				for( int indexX = 0; indexX < ConsoleWidth; indexX++ )
				{
					Console.SetCursorPosition( indexX, indexY );
					Console.Write( currentGrid[indexX, indexY] );
				}
			}
			Console.SetCursorPosition( 0, 0 );
		}

		private void _WriteGridToConsole()
		{
			char[,] currentGrid = SwitchGridFlag ? GameGrid : UpdatedGameGrid;
			char[,] otherGrid = SwitchGridFlag ? UpdatedGameGrid : GameGrid;

			Console.SetCursorPosition( 0, 0 );
			for( int indexY = 0; indexY < ConsoleHeight; indexY++ )
			{
				for( int indexX = 0; indexX < ConsoleWidth; indexX++ )
				{
					if( currentGrid[indexX, indexY] != otherGrid[indexX, indexY] )
					{
						Console.SetCursorPosition( indexX, indexY );
						Console.Write( currentGrid[indexX, indexY] );
					}
				}
			}
		}
	}
}
