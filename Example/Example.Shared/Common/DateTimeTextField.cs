using System;
using Qoden.Binding;
using Qoden.Reflection;
using Qoden.UI;

namespace Example
{
    public partial class DateTimeTextField : QodenView, ILabeledField
    {
        [View]
        public QEditText Text { get; protected set; }

        [View]
        public QTextView Label { get; protected set; }

        public float LabelWidth { get; set; }

        public string DateFormatString { get; set; } = "dd MMMM yy";

        DateTime _date = DateTime.Now;
        public DateTime Date 
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    if (value == DateTime.MinValue)
                    {
                        Text.SetText("");
                    }
                    else
                    {
                        Text.SetText(value.ToString(DateFormatString));    
                    }
                    _date = value;
                    DateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler DateChanged;

        private static readonly RuntimeEvent DateChangedEvent = new RuntimeEvent(typeof(DateTimeTextField), "DateChanged");
        private IPropertyBindingStrategy DateBinding = new EventHandlerBindingStrategy(DateChangedEvent);
        public IProperty<DateTime> DateProperty()
        {
            return new Property<DateTime>(this, "Date", DateBinding);
        }

        protected override void CreateView()
        {
            base.CreateView();
            //Text.SetEnabled(false);
            PlatformCreate();
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            this.LayoutLabeledField(layout);
        }

        private void OnClick(object sender, EventArgs e)
        {
            PlatformShowDatePicker();
        }
    }
}
