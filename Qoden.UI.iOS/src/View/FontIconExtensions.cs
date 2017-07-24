using System;
using UIKit;

namespace Qoden.UI
{
	public static class UILabelFontIconExtensions
	{
		public static void SetIcon(this UILabel label, FontIcon icon)
		{
            label.Font = new Font(icon.Name, icon.Size, icon.Style).ToFont();
			label.Text = icon.Icon.ToString();
		}

		public static void SetIcon(this UILabel label, FontIconTemplate template)
		{
			label.SetIcon(template.FontIcon((float)label.Font.PointSize));
		}
	}

	public static class UIButtonFontIconExtensions
	{
		public static void SetIcon(this UIButton btn, FontIcon icon, UIControlState state = UIControlState.Normal)
		{
			btn.Font = new Font(icon.Name, icon.Size, icon.Style).ToFont();
			btn.SetTitle(icon.Icon.ToString(), state);
		}

		public static void SetIcon(this UIButton btn, FontIconTemplate template, UIControlState state = UIControlState.Normal)
		{
			btn.SetIcon(template.FontIcon((float)btn.Font.PointSize), state);
		}
	}

	public static class UIBarButtonItemFontIconExtensions
	{
		public static void SetIcon(this UIBarButtonItem btn, FontIcon icon, UIColor color, UIControlState state = UIControlState.Normal)
		{
			var attrs = new UITextAttributes
			{
				Font = new Font(icon.Name, icon.Size, icon.Style).ToFont(),
				TextColor = color
			};
			btn.Title = icon.Icon.ToString();
			btn.SetTitleTextAttributes(attrs, state);
		}

		public static void SetIcon(this UIBarButtonItem btn, FontIconTemplate template, UIColor color, UIControlState state = UIControlState.Normal)
		{
			var attrs = btn.GetTitleTextAttributes(state);
			var size = attrs.Font != null ? attrs.Font.PointSize : 12;
			btn.SetIcon(template.FontIcon((float)size), color, state);
		}
	}
}
