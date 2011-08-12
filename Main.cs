using System;
using System.Collections;

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
			GCParser parser = new GCParser ("/Users/spencer/Projects/mezzo.nc");
			parser.WriteInstructions ();
		}
	}
}

