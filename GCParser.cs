using System;
using System.IO;
using System.Collections;

namespace GCodeShortener
{
	public class GCParser
	{
		public ArrayList instructionBlocks;
		public string filename; 
		// This block is temporarily here until I find a way to exclude M-blocks
		public InstructionBlock final;
		
		//
		public GCParser ()
		{
			instructionBlocks = new ArrayList ();
		}
		
		// 
		public GCParser (String filename)
		{
			// Instantiate all of the blocks
			instructionBlocks = new ArrayList ();
			
			Console.WriteLine ("Attempting to open " + filename);
			// Open up the file
			this.LoadFile (filename);
		}
		
		// Accepts a string filename and loads the contents into instructionBlocks
		public Boolean LoadFile (string filename)
		{
			this.filename = filename;
			using (StreamReader sr = new StreamReader (filename)) 
			{
				// The line in the file
				String line;
				InstructionBlock block = new InstructionBlock ();
				Instruction parent = null;
				int countLines = 0;
				while ((line = sr.ReadLine ()) != null) {
					++countLines;
					Instruction i = StringToInstruction (line);
					
					// Base case: it's the beginning of an instruction group
					if (i.type.Equals ("G00") && i.z.HasValue && i.z > 0) {
						if (block.instructions.Count > 0) {
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
				
				// Add the last block, if it's not already added
				if (! (block == null || instructionBlocks.Contains(block))) 
				{
					instructionBlocks.Add (block);
				}
				
				Console.WriteLine ("Read " + countLines + " lines.");
				return true;
			}
			
			return true;
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
			tempBlocks.RemoveAt (originIndex.Value + 1);
			
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
			// Add the final block with no neighbors
			orderedBlocks.Add (tempBlocks[0]);
			return orderedBlocks;
		}
		
		// Another distance-sorting method that tries to sort based on the 
		// end of the origin instruction, to the beginning of all destinations
		public ArrayList ShortestGaps ()
		{
			ArrayList tempBlocks = (ArrayList)instructionBlocks.Clone ();
			ArrayList orderedBlocks = new ArrayList ();
			while (tempBlocks.Count > 1)
			{
				InstructionBlock origin = (InstructionBlock)tempBlocks[0];
				tempBlocks.RemoveAt (0);
				orderedBlocks.Add (origin);
				InstructionBlock shortest = null;
				double distance = Double.PositiveInfinity;
				
				foreach (InstructionBlock neighbor in tempBlocks) 
				{
					if (origin.NewDistance (neighbor) < distance)
					{
						shortest = neighbor;
						distance = origin.NewDistance (neighbor);
					}
				}
				tempBlocks.Remove (shortest);
				tempBlocks.Insert (0, shortest);
			}

			// Add the final block with no neighbors
			orderedBlocks.Add (tempBlocks[0]);
			
			return orderedBlocks;		
		}
		
		// Accepts an arraylist of instructions (such as the one modified
		// by ShortestGaps) and returns the distance traveled.
		public int TotalDistance (ArrayList path)
		{
			double totalDistance = 0;
			for (int i = 1; i < path.Count; i++) 
			{
				totalDistance += ((InstructionBlock)path[i - 1]).NewDistance (((InstructionBlock)path[i]));
			}
			return (int) totalDistance;
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
						if (piece.Substring (1, 1).Equals ("-"))
							i.extra += piece + " ";
						else
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
						i.extra += piece + " ";
						break;
					}
				}
			}
			return i;
		}
		
		public void WriteInstructions ()
		{
			int lines = 0;
			int zebra = 0;
			//foreach (InstructionBlock i in this.ShortestGaps ())
			foreach (InstructionBlock i in instructionBlocks)
			{
				Console.Out.Write ("<pre style='margin:0px;");
				if (++zebra % 2 == 0) 
				{
					Console.Out.Write ("color:gray'>");
				} 
				else 
				{
					Console.Out.Write ("color:black'>");
				}
				foreach (Instruction j in i.instructions)
				{
					++lines;
					Console.Out.WriteLine (j.ToString ());
				}
				Console.Out.Write ("</pre>");
			}
			
			Console.Out.WriteLine ("Wrote " + lines + " lines");
		}
		
		public void WriteInstructions (string filename)
		{
			using (System.IO.StreamWriter outPath = new System.IO.StreamWriter (filename, true)) 
			{
				foreach (InstructionBlock i in instructionBlocks) 
				{
					foreach (Instruction j in i.instructions)
					{
						outPath.WriteLine (j.ToString ());
					}
				}
			}
		}
	}
}