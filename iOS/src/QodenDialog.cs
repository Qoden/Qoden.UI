using System;
using Foundation;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI
{
    public class QodenDialog<T> : QodenController<T> where T : UIView, new()
    {
        private UIViewController _parent;

        public QodenDialog()
        {
            Initialize();
        }

        public QodenDialog(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public QodenDialog(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public QodenDialog(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public QodenDialog(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
            Initialize();
        }

        private void Initialize()
        {
            ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
            //HideOnTap is true by default
            _tapRecognizer = new UITapGestureRecognizer(View_Tap);
            _tapRecognizer.CancelsTouchesInView = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (_tapRecognizer != null)
            {
                View.AddGestureRecognizer(_tapRecognizer);                
            }
        }

        public UIViewController Parent
        {
            get 
            {
                if (_parent == null)
                {
                    _parent = UIApplication.SharedApplication.KeyWindow.RootViewController;
                }
                return _parent;
            }
            set
            {
                _parent = Assert.Property(value).NotNull().Value;
            }
        }
        UITapGestureRecognizer _tapRecognizer;
        public bool HideOnTap 
        {
            get => _tapRecognizer != null;
            set 
            {
                if (value != HideOnTap)
                {
                    if (value)
                    {
                        _tapRecognizer = new UITapGestureRecognizer(View_Tap);
                        _tapRecognizer.CancelsTouchesInView = false;
                        View.AddGestureRecognizer(_tapRecognizer);
                    }
                    else
                    {
                        View.RemoveGestureRecognizer(_tapRecognizer);
                        _tapRecognizer.Dispose();
                        _tapRecognizer = null;
                    }
                }    
            }
        }

        public bool IsDisplayed
        {
            get;
            private set;
        }

        public event EventHandler WillShow;
        public event EventHandler DidShow;

        public event EventHandler WillHide;
        public event EventHandler DidHide;

        public void Show(bool animated = true)
        {
            WillShow?.Invoke(this, EventArgs.Empty);
            Parent.PresentViewController(this, animated, Dialog_DidShow);
            IsDisplayed = true;
        }

        public void Hide(bool animated = true)
        {
            if (IsDisplayed)
            {                
                WillHide?.Invoke(this, EventArgs.Empty);
                DismissViewController(animated, Dialog_DidHide);
                IsDisplayed = false;
            }
        }

        private void View_Tap()
        {
            if (HideOnTap) Hide(true);
        }

        private void Dialog_DidHide()
        {
            DidHide?.Invoke(this, EventArgs.Empty);
        }

        private void Dialog_DidShow()
        {
            DidShow?.Invoke(this, EventArgs.Empty);
        }
    }
}
