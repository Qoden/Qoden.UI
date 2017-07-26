using Qoden.UI;

namespace Example
{
    public partial class PhoneNumberField
    {
        private PhoneNumberFormatter _formatter;

        private void PlatformCreate()
        {
            _formatter = new PhoneNumberFormatter()
            {
                TextField = Text.PlatformView
            };
        }
    }
}
