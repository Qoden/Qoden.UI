using System;

namespace Qoden.UI
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

        public static RGB RGBA(byte red, byte green, byte blue, float alpha)
        {
            return new RGB(red, green, blue, (byte)Math.Round(255 * alpha));
        }

		public byte Red { get; private set; }

		public byte Green { get; private set; }

		public byte Blue { get; private set; }

		public byte Alpha { get; private set; }

        public static readonly RGB Clear = new RGB(0, 0, 0, 0);
	}
}