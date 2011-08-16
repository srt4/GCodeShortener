using System;
using System.Collections;
using System.Globalization;


namespace GCodeShortener
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ArrayList instructionArray = new ArrayList ();
			instructionArray.Add ("M04");
			instructionArray.Add ("G00 X1.0000 Y1.0000");
			instructionArray.Add ("G01 Z-0.2500");
			instructionArray.Add ("G01 X2.0000 Y1.0000");
			instructionArray.Add ("G01 X2.0000 Y2.0000");
			instructionArray.Add ("G01 X1.0000 Y2.0000");
			instructionArray.Add ("G01 X1.0000 Y1.0000");
			instructionArray.Add ("G00 Z1.000");
			instructionArray.Add ("M05");

			foreach (String instruction in instructionArray) 
			{
				//Console.Out.WriteLine(instruction);
			}
			
			
			GCParser parser = OpenFile ();
			Menu (parser);

		}
		
		private static GCParser OpenFile ()
		{
			Console.Write ("Please enter a file location: ");
			string filename = Console.ReadLine ();
			return new GCParser (filename);
		}
		
		private static void Menu (GCParser parser)
		{
			Console.Out.WriteLine ("What would you like to do?");
			Console.Out.WriteLine ("[O]ptimize the file using ShortestPath method (not working)");
			Console.Out.WriteLine ("Optimize the file using [S]hortestGaps method (working)");
			Console.Out.WriteLine ("[C]alculate the distance traveled in the file");
			Console.Out.WriteLine ("[P]rint out the set of instructions");
			Console.Out.WriteLine ("[W]rite the instructions to a file");
			
			string choice = Console.In.ReadLine ();
			switch (choice.ToLower ().Substring (0, 1)) {
			case "o":
				parser.instructionBlocks = parser.ShortestPath ();
				Console.Out.WriteLine ("The file is now optimized with ShortestPath");
				Menu (parser);
				break;
			case "s":
				parser.instructionBlocks = parser.ShortestGaps ();
				Console.Out.WriteLine ("The file is now optimized with ShortestGaps");
				Menu (parser);
				break;
			case "c":
				Console.Out.WriteLine ("The total distance traveled in this file is " + parser.TotalDistance (parser.instructionBlocks));
				Menu (parser);
				break;
			case "p":
				parser.WriteInstructions ();
				Menu (parser);
				break;
			case "w":
				Console.Write ("Output filename? (enter for " + parser.filename + ".optimized.nc) ");
				string path = Console.ReadLine ();
				if (path.Equals (""))
				{
					path = parser.filename + ".optimized.nc";
				}
				parser.WriteInstructions (path);
				break;
			}
		}
	}
}

