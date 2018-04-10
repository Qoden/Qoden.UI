using System;
using System.Linq;
using PointF = System.Drawing.PointF;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace Qoden.UI
{
    public class GradientShaderFactory : ShapeDrawable.ShaderFactory
    {
        private readonly float[] _locations;
        private readonly PointF _startPoint;
        private readonly PointF _endPoint;
        private readonly RGB[] _gradientColors;

        public GradientShaderFactory(
            float[] locations,
            PointF startPoint,
            PointF endPoint,
            RGB[] gradientColors)
        {
            _locations = locations;
            _startPoint = startPoint;
            _endPoint = endPoint;
            _gradientColors = gradientColors;
        }

        public override Shader Resize(int width, int height)
        {
            var linearGradient = new LinearGradient(
                _startPoint.X * width,
                _startPoint.Y * height,
                _endPoint.X * width,
                _endPoint.Y * height,
                _gradientColors.Select(rgb => rgb.ToColor().ToArgb()).ToArray(),
                _locations,
                Shader.TileMode.Repeat);
            return linearGradient;
        }
    }
}
