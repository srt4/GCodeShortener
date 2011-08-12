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
				
				// Keep track of the parent and the current block
				Instruction parent;
				InstructionBlock block = new InstructionBlock ();
				// Go line-by-line through the file
				while ((line = sr.ReadLine ()) != null) 
				{
					// Get the instruction object
					Instruction instruction = StringToInstruction (line);
					
					// Start spindle instruction - usually a parent
					if (instruction.type.Equals ("M04")) 
					{
						parent = instruction;
					}
					
					// Set the parent
					instruction.addParent (parent);
					
					// End spindle instruction
					if (instruction.type.Equals ("M05"))
					{
						parent = null;
						instructionBlocks.Add(block);
						block = new InstructionBlock ();
					}
					
					// Add to the block
					block.addInstruction (instruction);
				}
			}
		}
		
		// Returns an ordered array of the instructions sorted by distance between blocks
		public ArrayList ShortestPath (InstructionBlock origin)
		{
			// Find the index of the origin block
			int originIndex = instructionBlocks.IndexOf (origin);
			
			// Throw an error if it's not found
			if (originIndex < 0 || originIndex == null) 
			{
				throw new ArgumentException ("You did not specify a valid origin");
			}
			
			// Store the blocks that will be removed
			ArrayList tempBlocks = instructionBlocks.Clone ();
			tempBlocks.Insert (0, tempBlocks.RemoveAt (originIndex));
			
			// This is where the ordered blocks will go
			ArrayList orderedBlocks = new ArrayList ();
			
			// Starts at the origin, goes through each block
			while (tempBlocks.Count > 0)
			{
				// Take out the origin, and then insert it into sorted blocks
				InstructionBlock origin = tempBlocks.RemoveAt (0);
				orderedBlocks.Add (origin);
				
				// Set the current shortest distance to infinity
				int distance = Int32.MaxValue;
				InstructionBlock shortest;
				
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
			String[] pieces = line.Split (" ");

			switch (pieces.Length)
			{
			case 1:
				// Probably an M
				return new Instruction (pieces[0]);
			case 2:
				// Probably a Z
				return new Instruction (pieces[0], pieces[1]);
			case 3:
				// Probably an X,Y
				return new Instruction (pieces[0], pieces[1], pieces[2]);
			}
			
			// Should not get here if a proper string is given
			return null;
		}
	}
}