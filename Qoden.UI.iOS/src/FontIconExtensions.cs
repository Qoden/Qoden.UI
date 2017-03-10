using System;
using Qoden.UI.Platform.iOS;
using UIKit;

namespace Qoden.UI.iOS
{

	public static class UILabelFontIconExtensions
	{
		public static void SetIcon(this UILabel label, FontGlyph icon)
		{
			label.Font = icon.Font.UIFont();
			label.Text = icon.IconIndex.ToString();
		}

		public static void SetIcon(this UILabel label, FontIconTemplate template)
		{
			label.SetIcon(template.FontIcon((float)label.Font.PointSize));
		}
	}

	public static class UIButtonFontIconExtensions
	{
		public static void SetIcon(this UIButton btn, FontGlyph icon, UIControlState state = UIControlState.Normal)
		{
			btn.Font = icon.Font.UIFont();
			var f = btn.Font;
			var b = f.FamilyName;
			var c = f.PointSize;
			btn.SetTitle(icon.IconIndex.ToString(), state);
		}

		public static void SetIcon(this UIButton btn, FontIconTemplate template, UIControlState state = UIControlState.Normal)
		{
			btn.SetIcon(template.FontIcon((float)btn.Font.PointSize), state);
		}
	}

	public static class UIBarButtonItemFontIconExtensions
	{
		public static void SetIcon(this UIBarButtonItem btn, FontGlyph icon, UIColor color, UIControlState state = UIControlState.Normal)
		{
			var attrs = new UITextAttributes
			{
				Font = icon.Font.UIFont(),
				TextColor = color
			};
			btn.Title = icon.IconIndex.ToString();
			btn.SetTitleTextAttributes(attrs, state);
		}

		public static void SetIcon(this UIBarButtonItem btn, FontIconTemplate template, UIColor color, UIControlState state = UIControlState.Normal)
		{
			var attrs = btn.GetTitleTextAttributes(state);
			var size = attrs.Font != null ? attrs.Font.PointSize : 12;
			btn.SetIcon(template.FontIcon((float)size), color, state);
		}
	}

	public static class UITabBarItemFontIconExtensions
	{
		public static void SetIcon(this UITabBarItem tab, FontGlyph icon, IFontIconGenerator generator)
		{
			tab.Image = generator.CreateIcon(icon).UIImage();
		}

		public static void SetIcon(this UITabBarItem tab, FontIconTemplate template, IFontIconGenerator generator)
		{
			var attrs = tab.GetTitleTextAttributes(UIControlState.Normal);
			var size = attrs.Font.PointSize;
			tab.SetIcon(template.FontIcon((float)size), generator);
		}
	}
}
