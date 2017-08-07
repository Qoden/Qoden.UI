using System;
using Android.Views;
using Android.Widget;

namespace Qoden.UI
{
    public partial class QTextView : BaseView<TextView>
    {
        public override TextView Create(IViewHierarchyBuilder builder)
        {
            //This is to mimic iOS UILabel behavior which makes more sense as a default
            var view = (TextView)base.Create(builder);
            view.Gravity = Android.Views.GravityFlags.CenterVertical;
            return view;
        }
    }

    public static partial class TextViewExtensions
    {
        public static void SetFont(this TextView view, Font font)
        {
            view.Typeface = TypefaceCollection.Get(font.Name, font.Style);
            view.TextSize = font.Size;
        }

        public static void SetTextColor(this TextView view, RGB color)
        {
            view.SetTextColor(color.ToColor());
        }

        public static void SetText(this TextView view, string text)
        {
            view.Text = text;
        }

        public static string GetText(this TextView view)
        {
            return view.Text;
        }

        public static void SetTextAlignment(this TextView view, TextAlignment alignment)
        {
            switch(alignment)
            {
                case TextAlignment.Center:
                    view.Gravity = GravityFlags.Center;
                    break;
                case TextAlignment.Left:
                    view.Gravity = GravityFlags.Left;
                    break;
                case TextAlignment.Right:
                    view.Gravity = GravityFlags.Right;
                    break;
                default:
                    throw new ArgumentException(nameof(alignment));
            }
        }
    }
}
