using System;
using Qoden.Binding;
using UIKit;

namespace Qoden.UI
{
    public static class UISegmentedControlBindings
    {
        class SelectedSegment : Property<int>
        {
            public SelectedSegment(UISegmentedControl owner)
                : base(owner, "SelectedSegment", UIControlBindings.ValueBinding)
            {
            }

            protected override void SetValue(int value)
            {
                ((UISegmentedControl)Owner).SelectedSegment = value;
            }

            protected override int GetValue()
            {
                return (int)((UISegmentedControl)Owner).SelectedSegment;
            }
        }

        public static IProperty<int> SelectedSegmentProperty(this UISegmentedControl segments)
        {
            return new SelectedSegment(segments);
        }
    }
}
