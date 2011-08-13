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
		
		// * 10 ^ 4
		public override string ToString ()
		{
			string returnString = this.type + " ";
			if (this.x.HasValue)
			{
				//string = Math.Round(x.Value, MidpointRounding.AwayFromZero);
				returnString += "X" + FourDecimalDouble (this.x) + " ";
			}
			if (this.y.HasValue)
			{
				returnString += "Y" + FourDecimalDouble (this.y) + " ";
			}
			if (this.z.HasValue)
			{
				returnString += "Z" + FourDecimalDouble (this.z) + " ";
			}
			if (this.extra != null) 
			{
				returnString += this.extra + " ";
			}
			
			return returnString.TrimEnd ();
		}
		
		// The CNC-reading program doesn't like numbers without four decimals places. I don't blame it.
		private string FourDecimalDouble (double? dblParam)
		{
			double dbl = dblParam.Value;
			// Edge case: dbl == 0
			string negative = dbl < 0 ? "-" : "";
			
			dbl = Math.Abs (dbl);
			
			if (dbl == 0.0) 
			{
				return "0.0000";
			}
			
			double postDecimal = dbl * (Math.Pow (10, 4));
			string postString = postDecimal.ToString ();
			if (postString.Length > 4) 
			{
				return negative + postString.Substring (0, postString.Length - 4) + "." + 
					postString.Substring(postString.Length - 4);
			}
			else // postString length == 4 
			{
				string returnZeros = "";
				for (int i = 3; i >= postString.Length; i--)  
				{
					returnZeros += "0";
				}
				return negative + "0." + returnZeros + postString;
			}
		}
	}
}

