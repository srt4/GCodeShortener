using System;
using System.Collections;

namespace GCodeShortener
{
	public class InstructionBlock
	{
		ArrayList instructions;
		
		// Instantiates an empty instructionblock object
		public InstructionBlock ()
		{
			this.instructions = new ArrayList ();
		}
		
		// TODO: check and instruction block for integrity
		public Boolean addInstruction (Instruction instruction)
		{
			this.instructions.Add (instruction);
			return true;
		}
		
		// Empties the instructions
		public Boolean clear ()
		{
			this.instructions = new ArrayList ();
			return true;
		}
		
		// Returns all of the instructions
		public ArrayList getInstructions ()
		{
			return this.instructions;
		}

		// Takes the mean (x,y) of each instruction block,
		// and then finds the distance between them using 
		// a^2 + b^2 = c^2
		public double Distance (InstructionBlock other)
		{
			int a = this.MeanX () - other.MeanX ();
			int b = this.MeanY () - other.MeanY ();	
			int c = a ^ 2 + b ^ 2;
			return Math.Sqrt (c);
		}
		
		// Returns the mean x-coordinate of a set of instructions
		public int MeanX() 
		{
			ArrayList x = new ArrayList ();
			foreach (Instruction instruction in this.getInstructions ()) 
			{
				if (instruction.x != null && instruction.y != null) 
				{
					x.Add (instruction.x);
				}
			}
			return arrayMean (x);
		}
		
		// Returns the mean y-coordinate of a set of instructions
		public int MeanY()
		{
			ArrayList y = new ArrayList ();
			foreach (Instruction instruction in this.getInstructions ()) 
			{
				if (instruction.x != null && instruction.y != null) 
				{
					y.Add (instruction.y);
				}
			}	
			return arrayMean (y);
		}
		
		// This method takes an arraylist, and then
		// returns the mean of all the values in it
		private int arrayMean (ArrayList numbers)
		{
			int sum, count;
			count = sum = 0;
			
			foreach (int number in numbers) 
			{
				sum += number;
				count++;
			}
			
			return sum / count;
		}
	}
}

