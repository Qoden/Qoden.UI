using System;
using CoreGraphics;
using CoreText;
using UIKit;

namespace Qoden.UI
{
    public class FontIconAppearance
    {
        public FontIconAppearance(Font font)
        {
            UIKitFont = font.ToFont();
            CoreTextFont = font.ToCTFont();
            Colors = new[] { UIColor.DarkGray.CGColor };
            StrokeColor = UIColor.Black.CGColor;
            StrokeWidth = 0;
            RenderingMode = UIImageRenderingMode.Automatic;
        }

        public FontIconAppearance(CTFont coreTextFont,
                                  CGColor[] colors = null,
                                  UIImageRenderingMode renderingMode = UIImageRenderingMode.Automatic,
                                  CGColor strokeColor = null,
                                  float strokeWidth = 0)
        {
            CoreTextFont = coreTextFont;
            Colors = colors ?? new[] { UIColor.DarkGray.CGColor };
            RenderingMode = renderingMode;
            StrokeColor = strokeColor ?? UIColor.Black.CGColor;
            RenderingMode = renderingMode;
            StrokeWidth = strokeWidth;
        }

        CGColor[] _colors;
        public CGColor[] Colors
        {
            get => _colors;
            set { _colors = (value ?? throw new ArgumentNullException()); }
        }

        public UIImageRenderingMode RenderingMode { get; set; }

        CGColor _strokeColor;
        public CGColor StrokeColor
        { 
            get => _strokeColor; 
            set { _strokeColor = (value ?? throw new ArgumentNullException()); } 
        }

        float _strokeWidth;
        public float StrokeWidth
        {
            get => _strokeWidth;
            set { _strokeWidth = (value >= 0 ? value : throw new ArgumentException()); }
        }

        CTFont _coreTextFont;
        public CTFont CoreTextFont
        {
            get => _coreTextFont;
            set { _coreTextFont = (value ?? throw new ArgumentNullException()); }
        }
        
        UIFont _UIKitFont;
        public UIFont UIKitFont
        {
            get => _UIKitFont;
            set { _UIKitFont = (value ?? throw new ArgumentNullException()); }
        }
    }
}
