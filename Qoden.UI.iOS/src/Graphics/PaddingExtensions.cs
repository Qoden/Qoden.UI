﻿using UIKit;

namespace Qoden.UI
{
	public static class PaddingExtensions
	{
		public static EdgeInset AsPadding(this UIEdgeInsets p)
		{
			return new EdgeInset((float)p.Top, (float)p.Left, (float)p.Bottom, (float)p.Right);
		}

		public static UIEdgeInsets AsEdgeInsets(this EdgeInset p)
		{
			return new UIEdgeInsets(p.Top, p.Left, p.Bottom, p.Right);
		}
	}
}
