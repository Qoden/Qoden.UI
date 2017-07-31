using System;
using Qoden.Binding;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI
{
    public class UIViewClickEventSource : IEventSource<UIView>
    {
        UITapGestureRecognizer _tapRecognizer;

        public UIViewClickEventSource(UIView view)
        {
            Assert.Argument(view, nameof(view)).NotNull();
            Owner = view;
            _tapRecognizer = new UITapGestureRecognizer(OnTap);
            SetEnabled(true);
        }

        private void OnTap()
        {
            Handler?.Invoke(Owner, EventArgs.Empty);
        }

        object IEventSource.Owner => Owner;

        public UIView Owner { get; private set; }

        public Func<object, EventArgs, object> ParameterExtractor { get; set; }

        public event EventHandler Handler;

        public void SetEnabled(bool enabled)
        {
            if (enabled)
            {
                if (_tapRecognizer.View == null)
                {
                    Owner.AddGestureRecognizer(_tapRecognizer);
                }
            }
            else
            {
                if (_tapRecognizer.View != null)
                {
                    Owner.RemoveGestureRecognizer(_tapRecognizer);
                }
            }
        }
    }
}
