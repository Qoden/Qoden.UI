using System;

namespace Qoden.UI
{
	public struct Padding
	{
		public float Left;
		public float Top;
		public float Right;
		public float Bottom;

		public Padding(float l, float t, float r, float b)
		{
			Left = l;
			Top = t;
			Right = r;
			Bottom = b;
		}

		public static readonly Padding Zero = new Padding(0, 0, 0, 0);
	}
}

