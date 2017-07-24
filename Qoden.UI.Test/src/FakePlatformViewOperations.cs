using System;
using System.Drawing;
using System.Reflection;

namespace Qoden.UI.Test
{
    public class FakePlatformViewOperations : IPlatformViewOperations
    {
        public void AddSubview(PlatformView parent, PlatformView child)
        {
            (parent.Native as FakeView).Subviews.Add(child.Native as FakeView);
        }

        public PlatformView CreateView(PlatformView? parent, Type type)
        {
            return new PlatformView(new FakeView()
            {
                ViewType = type
            });
        }

        public RectangleF Frame(PlatformView view)
        {
            return (view.Native as FakeView).Frame;
        }

        public bool IsView(object nativeView)
        {
            return nativeView is FakeView;
        }

        public IViewLayoutBox MakeViewLayoutBox(PlatformView view, RectangleF bounds)
        {
            return new FakeViewLayoutBox(view.Native as FakeView, bounds);
        }

        public EdgeInset LayoutMargins(PlatformView view)
        {
            return (view.Native as FakeView).LayoutMargins;
        }

        public SizeF Measure(PlatformView platformView, SizeF parentSize)
        {
            return (platformView.Native as FakeView).Frame.Size;
        }

        public void RemoveSubview(PlatformView parent, PlatformView child)
        {
            (parent.Native as FakeView).Subviews.Remove(child.Native as FakeView);
        }

        public void SetFrame(PlatformView view, RectangleF frame)
        {
            (view.Native as FakeView).Frame = frame;
        }

        public void SetLayoutMargins(PlatformView native, EdgeInset value)
        {
            (native.Native as FakeView).LayoutMargins = value;
        }

        public static void Install(IPlatformViewOperations operations)
        {
            var setOperations = typeof(PlatformView).GetTypeInfo().GetProperty("Operations");
            setOperations.SetValue(null, operations);
        }

        public void AddSubview(object parent, object subview)
        {
            throw new NotImplementedException();
        }
    }
}
