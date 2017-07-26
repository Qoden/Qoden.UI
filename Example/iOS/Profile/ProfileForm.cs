using System;
using CoreGraphics;
using Qoden.UI;
using UIKit;

namespace Example
{
    public partial class ProfileForm
    {
        KeyboardScroller _scroller;
        const int TopInset = 20;

        private void PlatformCreate()
        {
            _scroller = new KeyboardScroller()
            {
                AutoHideKeyboardOffset = 60
            };
            _scroller.ScrollView = Form.PlatformView;
        }

        private void Form_Scrolled()
        {
            _scroller.ScrollViewDidScroll();
        }

        private void PlatformDispose()
        {
            _scroller?.Dispose();
            _scroller = null;
        }
    }
}
