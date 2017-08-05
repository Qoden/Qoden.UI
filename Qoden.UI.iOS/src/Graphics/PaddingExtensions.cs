﻿using UIKit;

namespace Qoden.UI
{
	public static class PaddingExtensions
	{
		public static EdgeInsets AsPadding(this UIEdgeInsets p)
		{
			return new EdgeInsets((float)p.Top, (float)p.Left, (float)p.Bottom, (float)p.Right);
		}

		public static UIEdgeInsets AsEdgeInsets(this EdgeInsets p)
		{
			return new UIEdgeInsets(p.Top, p.Left, p.Bottom, p.Right);
		}
	}
}
