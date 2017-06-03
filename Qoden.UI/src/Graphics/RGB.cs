namespace Qoden.View
{
	public struct RGB
	{
		public RGB (byte red, byte green, byte blue) : this()
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = 0xff;
		}

		public RGB (byte red, byte green, byte blue, byte alpha) : this()
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		public byte Red{ get; private set; }

		public byte Green { get; private set; }

		public byte Blue { get; private set; }

		public byte Alpha { get; private set; }

		public static readonly RGB White = new RGB(byte.MaxValue, byte.MaxValue, byte.MaxValue);
		public static readonly RGB Black = new RGB(0, 0, 0);
		public static readonly RGB DarkGray = new RGB(0x55, 0x55, 0x55);
	}
}