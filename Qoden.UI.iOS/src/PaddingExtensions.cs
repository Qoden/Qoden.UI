using UIKit;

namespace Qoden.UI.Platform.iOS
{

	public static class PaddingExtensions
	{
		public static Padding AsPadding(this UIEdgeInsets p)
		{
			return new Padding((float)p.Top, (float)p.Left, (float)p.Bottom, (float)p.Right);
		}

		public static UIEdgeInsets AsEdgeInsets(this Padding p)
		{
			return new UIEdgeInsets(p.Top, p.Left, p.Bottom, p.Right);
		}
	}

}
