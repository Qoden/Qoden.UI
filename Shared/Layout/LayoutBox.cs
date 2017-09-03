using System;
using System.Drawing;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{    
    public class LayoutBox : ILayoutBox
    {
        private float _left;
        private float _right;
        private float _width;
        private float _centerX;
        private float _top;
        private float _bottom;
        private float _height;
        private float _centerY;

        public float Left => MarginLeft;
        public float Right => MarginLeft + Width;
        public float Top => MarginTop;
        public float Bottom => MarginTop + Height;

        public RectangleF Bounds => new RectangleF(Left, Top, Width, Height);
        public SizeF Size => new SizeF(Width, Height);
        public RectangleF Frame => new RectangleF(Left + OuterBounds.Left, Top + OuterBounds.Top, Width, Height);
        
        public float MarginLeft
        {
            get
            {
                if (IsSet(_left))
                    return _left;
                if (IsSet(_centerX) && IsSet(_width))
                    return _centerX - _width / 2;
                if (IsSet(_right) && IsSet(_width))
                    return OuterBounds.Width - _right - _width;
                return 0;
            }
            set
            {
                if (IsSet (_centerX) && IsSet (_width))
                    _centerX = NotSet;
                if (IsSet (_right) && IsSet (_width))
                    _right = NotSet;
                _left = value;
            }
        }

        public float MarginRight
        {
            get
            {
                if (IsSet(_right))
                    return _right;
                if (IsSet(_left) && IsSet(_width))
                    return OuterBounds.Width - (_left + _width);
                if (IsSet(_centerX) && IsSet(_width))
                    return OuterBounds.Width - (_centerX + _width / 2);
                return 0;
            }
            
            set
            {
                if (IsSet (_centerX) && IsSet (_width))
                    _centerX = NotSet;
                if (IsSet (_left) && IsSet (_width))
                    _width = NotSet;
                _right = value;
            }
        }
        
        public float MarginTop
        {
            get
            {
                if (IsSet(_top))
                    return _top;
                if (IsSet(_centerY) && IsSet(_height))
                    return _centerY - _height / 2;
                if (IsSet(_bottom) && IsSet(_height))
                    return OuterBounds.Height - _bottom - _height;
                return 0;
            }
            set
            {
                if (IsSet (_centerY) && IsSet (_height))
                    _centerY = NotSet;
                if (IsSet (_bottom) && IsSet (_height))
                    _bottom = NotSet;
                _top = value;
            }
        }

        public float MarginBottom
        {
            get
            {
                if (IsSet(_bottom))
                    return _bottom;
                if (IsSet(_top) && IsSet(_height))
                    return OuterBounds.Height - _top - _height;
                if (IsSet(_centerY) && IsSet(_height))
                    return OuterBounds.Height - (_centerY + _height / 2);
                return 0;
            }
            set
            {
                if (IsSet (_centerY) && IsSet (_height))
                    _centerY = NotSet;
                if (IsSet (_top) && IsSet (_height))
                    _height = NotSet;
                _bottom = value;
            }
        }

        public float Width
        {
            get
            {
                if (IsSet(_width))
                    return _width;
                if (IsSet(_centerX) && IsSet(_left))
                    return (_centerX - _left) * 2;
                if (IsSet(_centerX) && IsSet(_right))
                    return (OuterBounds.Width - _right - _centerX) * 2;
                if (IsSet(_left) && IsSet(_right))
                    return OuterBounds.Width - _right - _left;
                return OuterBounds.Width;
            }
            set
            {
                if (IsSet (_left) && IsSet (_right)) {
                    _right = NotSet;
                }
                if (IsSet (_centerX) && IsSet (_left))
                    _centerX = NotSet;
                if (IsSet (_centerX) && IsSet (_right))
                    _right = NotSet;
                _width = value;
            }
        }
        
        public float Height
        {
            get
            {
                if (IsSet(_height))
                    return _height;
                if (IsSet(_top) && IsSet(_bottom))
                    return OuterBounds.Height - _bottom - _top;
                if (IsSet(_centerY) && IsSet(_top))
                    return (_centerY - _top) * 2;
                if (IsSet(_centerY) && IsSet(_bottom))
                    return (OuterBounds.Height - _bottom - _centerY) * 2;
                return OuterBounds.Height;
            }
            set
            {
                if (IsSet (_top) && IsSet (_bottom)) {
                    _bottom = NotSet;
                }
                if (IsSet (_centerY) && IsSet (_top))
                    _centerY = NotSet;
                if (IsSet (_centerY) && IsSet (_bottom))
                    _bottom = NotSet;
                _height = value;
            }
        }
        
        public float CenterX
        {
            get => MarginLeft + Width/2;
            set
            {
                if (IsSet (_left) && IsSet (_right))
                    _right = NotSet;
                if (IsSet (_left) && IsSet (_width))
                    _width = NotSet;
                if (IsSet (_right) && IsSet (_width))
                    _right = NotSet;
                _centerX = value;
            }
        }
        
        public float CenterY
        {
            get => MarginTop + Height/2;
            set
            {
                if (IsSet (_top) && IsSet (_bottom))
                    _bottom = NotSet;
                if (IsSet (_top) && IsSet (_height))
                    _height = NotSet;
                if (IsSet (_bottom) && IsSet (_height))
                    _bottom = NotSet;
                _centerY = value;
            }
        }

        private const float NotSet = float.MaxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Qoden.UI.LayoutBox"/> class.
        /// </summary>
        /// <param name="outerBounds">Layout bounds in view coordinate system in pixels</param>
        public LayoutBox(RectangleF outerBounds) : this(outerBounds, Units.PlatformDefault)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Qoden.UI.LayoutBox"/> class.
        /// </summary>
        /// <param name="outerBounds">Layout bounds in view coordinate system in pixels</param>
        /// <param name="unit">Unit to be used for relative offsets</param>
        public LayoutBox(RectangleF outerBounds, IUnit unit)
        {
            OuterBounds = outerBounds;
            MarginLeft = MarginRight = MarginTop = MarginBottom = Width = Height = CenterX = CenterY = NotSet;
            Unit = unit ?? Units.PlatformDefault;
        }

        /// <summary>
        /// Measurement unit for relative offsets
        /// </summary>
        public IUnit Unit { get; }

        public static bool IsSet(float val)
        {
            return Math.Abs(val - NotSet) > float.Epsilon;
        }

        public RectangleF OuterBounds { get; }

        public override string ToString()
        {
            var rect = Frame;
            return $"L={rect.Left}, T={rect.Top}, R={rect.Right}, B={rect.Bottom}";
        }
    }
}

