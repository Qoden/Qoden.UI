using System.Drawing;
#if __IOS__
using UIKit;
#endif

namespace Qoden.UI
{
    public struct EdgeInsets
    {
        public float Left { get; }
        public float Top { get; }
        public float Right { get; }
        public float Bottom { get; }

        public EdgeInsets(float left = 0, float top = 0, float right = 0, float bottom = 0, IUnit unit = null)
        {
            unit = unit ?? Units.Id;

            Left = unit.ToPixels(left);
            Top = unit.ToPixels(top);
            Right = unit.ToPixels(right);
            Bottom = unit.ToPixels(bottom);
        }

        public static readonly EdgeInsets Zero = new EdgeInsets();

        public RectangleF Substract(RectangleF rect)
        {
            return new RectangleF(rect.Left - Left, rect.Top - Top, rect.Width - Left - Right,
                rect.Height - Top - Bottom);
        }

        public bool Equals(EdgeInsets other)
        {
            return Left.Equals(other.Left) && Top.Equals(other.Top) && Right.Equals(other.Right) &&
                   Bottom.Equals(other.Bottom);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is EdgeInsets && Equals((EdgeInsets) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left.GetHashCode();
                hashCode = (hashCode * 397) ^ Top.GetHashCode();
                hashCode = (hashCode * 397) ^ Right.GetHashCode();
                hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
                return hashCode;
            }
        }
    }

    public static class PaddingExtensions
    {
#if __IOS__
        public static EdgeInsets ToEdgeInsets(this UIEdgeInsets p)
        {
            return new EdgeInsets((float) p.Top, (float) p.Left, (float) p.Bottom, (float) p.Right);
        }

        // ReSharper disable once InconsistentNaming
        public static UIEdgeInsets ToUIEdgeInsets(this EdgeInsets p)
        {
            return new UIEdgeInsets(p.Top, p.Left, p.Bottom, p.Right);
        }
#endif
    }
}