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

        public float Left
        {
            get => _left;
            set
            {
                if (IsSet (_centerX) && IsSet (_width))
                    _centerX = NotSet;
                if (IsSet (_right) && IsSet (_width))
                    _width = NotSet;
                _left = value;
            }
        }

        public float Right
        {
            get => _right;
            set
            {
                if (IsSet (_centerX) && IsSet (_width))
                    _centerX = NotSet;
                if (IsSet (_left) && IsSet (_width))
                    _width = NotSet;
                _right = value;
            }
        }
        
        public float Top
        {
            get => _top;
            set
            {
                if (IsSet (_centerY) && IsSet (_height))
                    _centerY = NotSet;
                if (IsSet (_bottom) && IsSet (_height))
                    _height = NotSet;
                _top = value;
            }
        }

        public float Bottom
        {
            get => _bottom;
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
            get => _width;
            set
            {
                if (IsSet (_left) && IsSet (_right)) {
                    _left = NotSet;
                }
                if (IsSet (_centerX) && IsSet (_left))
                    _centerX = NotSet;
                if (IsSet (_centerX) && IsSet (_right))
                    _centerX = NotSet;
                _width = value;
            }
        }
        
        public float Height
        {
            get => _height;
            set
            {
                if (IsSet (_top) && IsSet (_bottom)) {
                    _top = NotSet;
                }
                if (IsSet (_centerY) && IsSet (_top))
                    _centerY = NotSet;
                if (IsSet (_centerY) && IsSet (_bottom))
                    _centerY = NotSet;
                _height = value;
            }
        }
        
        public float CenterX
        {
            get => _centerX;
            set
            {
                if (IsSet (_left) && IsSet (_right)) {
                    _width = OuterBounds.Width - _right - _left;
                    _left = _right = NotSet;
                }
                if (IsSet (_left) && IsSet (_width))
                    _width = NotSet;
                if (IsSet (_right) && IsSet (_width))
                    _width = NotSet;
                _centerX = value;
            }
        }
        
        public float CenterY
        {
            get => _centerY;
            set
            {
                if (IsSet (_top) && IsSet (_bottom)) {
                    _height = OuterBounds.Height - _bottom - _top;
                    _top = _bottom = NotSet;
                }
                if (IsSet (_top) && IsSet (_height))
                    _height = NotSet;
                if (IsSet (_bottom) && IsSet (_height))
                    _height = NotSet;
                _centerY = value;
            }
        }

        const float NotSet = float.MaxValue;

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
            Left = Right = Top = Bottom = Width = Height = CenterX = CenterY = NotSet;
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
            var rect = this.Frame();
            return $"[LayoutBox: Left={rect.Left}, Top={rect.Top}, Right={rect.Right}, Bottom={rect.Bottom}";
        }
    }
}

