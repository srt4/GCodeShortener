// Simple class to eliminate the need for *X and *Y variables/methods.

using System;
namespace GCodeShortener
{
	public class Point
	{
		public double x;
		public double y;
		
		public Point (double? x, double? y)
		{
			this.x = x.Value;
			this.y = y.Value;
		}
	}
}

