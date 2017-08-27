using System;
using Qoden.Binding;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI
{
    public class UiViewClickCommandTrigger : ICommandTrigger
    {
        readonly UITapGestureRecognizer _tapRecognizer;

        public UiViewClickCommandTrigger(UIView view)
        {
            Assert.Argument(view, nameof(view)).NotNull();
            Owner = view;
            _tapRecognizer = new UITapGestureRecognizer(OnTap);
            SetEnabled(true);
        }

        private void OnTap()
        {
            Trigger?.Invoke(null);
        }

        public UIView Owner { get; }

        public Func<object, EventArgs, object> ParameterExtractor { get; set; }

        public event CommandHandler Trigger;

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
