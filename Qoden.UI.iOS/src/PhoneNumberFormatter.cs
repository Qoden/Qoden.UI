using System;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public class PhoneNumberFormatter : IDisposable
    {
        PhoneAsYouTypeFormatter _formatter;

        public PhoneNumberFormatter(string locale = "US")
        {
            _formatter = new PhoneAsYouTypeFormatter(locale);
        }

        UITextField _field;
        private IDisposable _kvc;

        public UITextField TextField
        {
            get => _field;
            set
            {
                if (_field != null)
                {
                    _field.ShouldChangeCharacters = null;
                    _kvc?.Dispose();
                }
                if (_field != value)
                {
                    _field = value;
                    if (_field != null)
                    {
                        _field.KeyboardType = UIKeyboardType.PhonePad;
                        _field.ShouldChangeCharacters = PhoneNumber_ShouldChangeCharacters;
                        _kvc = _field.AddObserver("text", NSKeyValueObservingOptions.OldNew, TextField_TextChanged);
                    }
                }
            }
        }

        bool changingText = false;
        private void TextField_TextChanged(NSObservedChange change)
        {
            if (!changingText && !change.NewValue.IsEqual(change.OldValue))
            {
                if (_formatter.Output != _field.Text)
                {
                    changingText = true;
                    _field.Text = _formatter.Reset(_field.Text);
                    changingText = false;
                }
            }
        }

        public void Dispose()
        {
            if (_field != null)
            {
                TextField = null;
            }
        }

        bool PhoneNumber_ShouldChangeCharacters(UITextField tf, NSRange range, string chars)
        {
            if (_formatter.Output != tf.Text)
            {
                tf.Text = _formatter.Reset(tf.Text);
            }
            else
            {
                var formattedText = tf.Text;
                var newPhoneNumberCursorPos = CountPhoneNumberChars(formattedText, (int)range.Location) + CountPhoneNumberChars(chars, chars.Length);
                formattedText = _formatter.Replace((int)range.Location, (int)range.Length, chars);
                tf.Text = formattedText;
                var newCursorOffset = FormattedTextCursorPosition(formattedText, newPhoneNumberCursorPos);
                var cursorLocation = tf.GetPosition(tf.BeginningOfDocument, newCursorOffset);
                if (cursorLocation != null)
                {
                    tf.SelectedTextRange = tf.GetTextRange(cursorLocation, cursorLocation);
                }
            }
            return false;
        }

        private int CountPhoneNumberChars(string chars, int limit)
        {
            int result = 0;
            for (int i = 0; i < limit; ++i)
            {
                if (_formatter.IsPhoneNumberDigit(chars[i]))
                    result++;
            }
            return result;
        }

        private int FormattedTextCursorPosition(string formattedText, int phoneNumberCursorPosition)
        {
            var newCursorOffset = 0;
            while (newCursorOffset < formattedText.Length && phoneNumberCursorPosition > 0)
            {
                if (_formatter.IsPhoneNumberDigit(formattedText[newCursorOffset]))
                {
                    phoneNumberCursorPosition--;
                }
                newCursorOffset++;
            }
            return newCursorOffset;
        }
    }
}
