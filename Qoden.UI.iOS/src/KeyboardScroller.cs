using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Qoden.UI.iOS
{
    /// <summary>
    /// Helper component to auto scroll scroll view when keyboard popup's
    /// Typical usage
    /// <code>
    ///keyboardController = new KeyboardScroller {
    ///				ScrollView = MyScrollView
    ///}
    /// </code>
    /// And then
    /// <code>
    /// protected override Dispose(bool disposing) {
    ///   if (disposing) {
    ///     keyboardController.Dispose();
    ///   }
    /// }
    /// </code>
    /// 
    /// </summary>
    public class KeyboardScroller : IDisposable
    {
        private NSObject keyboardWillShow, keyboardDidShow, keyboardWillHide;
        private NSObject textFieldBeginEditing, textFieldEndEditing;
        private NSObject textViewBeginEditing, textViewEndEditing;

        public KeyboardScroller()
        {
            keyboardWillShow = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardWillShow);
            keyboardDidShow = UIKeyboard.Notifications.ObserveDidShow(OnKeyboardDidShow);
            keyboardWillHide = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardWillHide);

            textFieldBeginEditing = UITextField.Notifications.ObserveTextDidBeginEditing(DidBeginEditing);
            textFieldEndEditing = UITextField.Notifications.ObserveTextDidEndEditing(DidEndEditing);

            textViewBeginEditing = UITextView.Notifications.ObserveTextDidBeginEditing(DidBeginEditing);
            textViewEndEditing = UITextView.Notifications.ObserveTextDidEndEditing(DidEndEditing);
        }

        public void Dispose()
        {
            keyboardWillShow.Dispose();
            keyboardDidShow.Dispose();
            keyboardWillHide.Dispose();

            textFieldBeginEditing.Dispose();
            textFieldEndEditing.Dispose();

            textViewBeginEditing.Dispose();
            textViewEndEditing.Dispose();
        }

        private void DidBeginEditing(object sender, NSNotificationEventArgs e)
        {
            if (ScrollView != null)
            {
                var view = e.Notification.Object as UIView;
                var parent = view.Superview;
                while (parent != null)
                {
                    if (parent.Handle == ScrollView.Handle)
                    {
                        ActiveView = view;
                    }
                    parent = parent.Superview;
                }
            }
        }

        private void DidEndEditing(object sender, NSNotificationEventArgs e)
        {
            ActiveView = null;
        }

        public UIView ActiveView
        {
            get;
            set;
        }

        public UIScrollView ScrollView
        {
            get;
            set;
        }

        private void OnKeyboardWillShow(object sender, UIKeyboardEventArgs args)
        {
            if (ScrollView != null)
            {
                var orientation = UIApplication.SharedApplication.StatusBarOrientation;

                var kbRect = args.FrameEnd;
                if (orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight)
                {
                    var origKeySize = kbRect.Size;
                    kbRect.Height = origKeySize.Width;
                    kbRect.Width = origKeySize.Height;
                }

                UIEdgeInsets contentInsets;
                CGRect translatedView = ScrollView.ConvertRectFromView(kbRect, UIApplication.SharedApplication.Delegate.GetWindow());
                CGRect intersection = CGRect.Intersect(ScrollView.Bounds, translatedView);
                contentInsets = new UIEdgeInsets(0.0f, 0.0f, intersection.Height, 0.0f);

                ScrollView.ContentInset = contentInsets;
                ScrollView.ScrollIndicatorInsets = contentInsets;
            }
        }

        private void OnKeyboardDidShow(object sender, UIKeyboardEventArgs args)
        {
            // If active text field is hidden by keyboard, scroll it so it's visible
            // Your application might not need or want this behavior.
            var kbRect = args.FrameEnd;
            if (ActiveView != null && ActiveView.Superview != null)
            {
                var keyboardRect = ActiveView.Window.ConvertRectToView(kbRect, ActiveView.Superview);
                if (keyboardRect.IntersectsWith(ActiveView.Frame))
                {
                    var shift = ActiveView.Frame.Bottom - keyboardRect.Top;
                    var offset = ScrollView.ContentOffset;
                    var newOffset = new CGPoint(offset.X, offset.Y + shift + 5);
                    ScrollView.SetContentOffset(newOffset, true);
                }
            }
        }

        private void OnKeyboardWillHide(object sender, UIKeyboardEventArgs args)
        {
            if (ScrollView != null)
            {
                UIEdgeInsets contentInsets = UIEdgeInsets.Zero;
                ScrollView.ContentInset = contentInsets;
                ScrollView.ScrollIndicatorInsets = contentInsets;
            }
        }
    }
}

