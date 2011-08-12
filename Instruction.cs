using System;
namespace GCodeShortener
{
	public class Instruction
	{
		// Public, private fields
		public Instruction parent;
		public double? x;
		public double? y;
		public double? z;
		public String type;
		public String extra;
		
		// Should not be able to construct an empty instruction
		public Instruction ()
		{
		}
		
		// Accepts a type
		public Instruction (String type)
		{
			this.type = type;
		}
		
		// Accepts a type, and potential z-coordinate for an instruction
		public Instruction (String type, double z)
		{
			this.type = type;
			this.z = z;
		}
		
		// Accepts a type, and x,y-coordinates for an instruction
		public Instruction (String type, double x, double y)
		{
			this.type = type;
			this.x = x;
			this.y = y;
		}
		
		// Set z-value; throw an exception if it's specified with an x or y
		public void setZ (double z)
		{
			this.z = z;
		}
		
		// Set x-value; throw exception if paired with z
		public void setX (double x)
		{
			this.x = x;
		}
		
		// Set y-value; throw exception if paired with z
		public void setY (double y)
		{
			this.y = y;
		}
		
		// Set type; throw an exception if M is paired with coordinates
		public void setType (String type)
		{
			if (type.ToLower().StartsWith ("M")) 
			{
				if (this.x.HasValue || this.y.HasValue || this.z.HasValue) 
				{
					throw new ArgumentException ("Type 'M' does not accept coordinates");
				}
			}
			this.type = type;
		}
		
		// Returns the first character of type (i.e., without the number) 
		public string getParentType ()
		{
			return this.type.Substring (0, 1);
		}
		
		// Instructions /should/ have a parent, unless they are of type "M"
		public void addParent (Instruction parent)
		{
			if (this.type.ToLower().Equals ("m04")) 
			{
				throw new ArgumentException ("The instruction of type M cannot be called with a parent");
			}
			this.parent = parent;
		}
		
		public override string ToString ()
		{
			string returnString = this.type + " ";
			if (this.x.HasValue)
			{
				returnString += "X" + this.x.Value.ToString () + " ";
			}
			if (this.y.HasValue)
			{
				returnString += "Y" + this.y.Value.ToString () + " ";
			}
			if (this.z.HasValue)
			{
				returnString += "Z" + this.z.Value.ToString () + " ";
			}
			if (this.extra != null) 
			{
				returnString += this.extra + " ";
			}
			
			return returnString;
		}
	}
}

