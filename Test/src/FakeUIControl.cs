using System;
using System.ComponentModel;
using Qoden.Binding;
using Qoden.Util;

namespace Qoden.UI.Test
{
    public class FakeUiControl : INotifyPropertyChanged
    {
        string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
                TextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> TextChanged;

        public IProperty<string> TextProperty()
        {
            return this.GetProperty(_ => _.Text, TextChangedBinding);
        }
        public static readonly RuntimeEvent TextChangedEvent = new RuntimeEvent(typeof(FakeUiControl), nameof(TextChanged));
        public static readonly IPropertyBindingStrategy TextChangedBinding = new EventHandlerBindingStrategy<EventArgs>(TextChangedEvent);

        public event EventHandler FakeControlClick;
        public static readonly RuntimeEvent ButtonClickEvent = new RuntimeEvent(typeof(FakeUiControl), nameof(FakeControlClick));
        public ICommandTrigger ButtonClickTrigger()
        {
            return new EventCommandTrigger(ButtonClickEvent, this);
        }

        public void TriggerClick()
        {
            FakeControlClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
