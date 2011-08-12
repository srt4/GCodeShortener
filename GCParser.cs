using System;
using System.IO;
using System.Collections;

namespace GCodeShortener
{
	public class GCParser
	{
		public ArrayList instructionBlocks;
		
		public GCParser ()
		{
		}
		
		public GCParser (String filename)
		{
			// Instantiate all of the blocks
			instructionBlocks = new ArrayList ();
			
			// Open up the file
			using (StreamReader sr = new StreamReader (filename)) 
			{
				// The line in the file
				String line;
				InstructionBlock block = new InstructionBlock ();
				Instruction parent = null;
				while ((line = sr.ReadLine ()) != null) 
				{
					Instruction i = StringToInstruction (line);
					
					// Base case: it's the beginning of an instruction group
					if (i.type.Equals ("G00") && i.z.HasValue && i.z > 0) 
					{
						if (block.instructions.Count > 0) 
						{
							instructionBlocks.Add (block);
						}
						block = new InstructionBlock ();
						parent = i;
					}
					
					// Set parent, if it's not null
					if (parent != null)
						i.addParent (i);
					
					// Finally, add the instruction to the block
					block.addInstruction (i);
				}
			}
		}
		
		// The origin is the first block found in the file
		public ArrayList ShortestPath ()
		{
			return ShortestPath ((InstructionBlock) instructionBlocks[8]);
		}
		
		// Returns an ordered array of the instructions sorted by distance between blocks
		public ArrayList ShortestPath (InstructionBlock origin)
		{
			// Find the index of the origin block
			int? originIndex = instructionBlocks.IndexOf (origin);
			
			// Throw an error if it's not found
			if (originIndex < 0 || originIndex == null) 
			{
				throw new ArgumentException ("You did not specify a valid origin");
			}
			
			// Store the blocks that will be removed
			ArrayList tempBlocks = (ArrayList)instructionBlocks.Clone ();
			tempBlocks.Insert (0, tempBlocks[originIndex.Value]);
			tempBlocks.RemoveAt (originIndex.Value);
			
			// This is where the ordered blocks will go
			ArrayList orderedBlocks = new ArrayList ();
			
			// Starts at the origin, goes through each block
			while (tempBlocks.Count > 1)
			{
				// Take out the origin, and then insert it into sorted blocks
				origin = (InstructionBlock)tempBlocks[0];
				tempBlocks.RemoveAt (0);
				orderedBlocks.Add (origin);
				
				// Set the current shortest distance to infinity
				double distance = Double.PositiveInfinity;
				InstructionBlock shortest = null;
				
				// Examine each reamining neighbor ... if it's shorter, it's the new destination
				foreach (InstructionBlock destination in tempBlocks) 
				{
					// If a closer block is found, it's the new destination
					if (origin.Distance (destination) < distance) 
					{
						shortest = destination;
					}
				}
				
				// Remove the shortest block, add it to the front
				tempBlocks.Remove (shortest);
				tempBlocks.Insert (0, shortest);
			}
			
			return orderedBlocks;
		}
		
		// Takes a string, such as "G01 X2.0000 Y1.0000", and converts 
		// it to an instruction object
		public Instruction StringToInstruction (String line)
		{
			// Split on whitespace
			String[] pieces = line.Split (' ');
			Instruction i = new Instruction ();
			
			foreach (String piece in pieces) 
			{
				if (piece.Length > 1) 
				{
					string pieceType = piece.Substring (0, 1);
					double pieceValue = Double.Parse (piece.Substring (1));
					switch (Char.Parse (pieceType)) {
					case 'M':
						i.setType (piece);
						break;
					case 'G':
						i.setType (piece);
						break;
					case 'X':
						i.setX (pieceValue);
						break;
					case 'Y':
						i.setY (pieceValue);
						break;
					case 'Z':
						i.setZ (pieceValue);
						break;
					default:
						i.extra = piece;
						break;
					}
				}
			}
			return i;
		}
		
		public void WriteInstructions ()
		{
			foreach (InstructionBlock i in this.ShortestPath ())
			{
				foreach (Instruction j in i.instructions)
				{
					Console.Out.WriteLine (j.ToString ());
				}
			}
		}
		
		public void WriteInstructions (string filename)
		{
		}
	}
}