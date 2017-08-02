using System;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public class PhoneNumberFormatter : IDisposable
    {
        //When AsYouTypeFormatter created first time it is very slow (2 sec on Mac Book Pro)
        static bool slowInitDone;

        public static void Warm()
        {
            new System.Threading.Thread(() =>
            {
                var vv = new PhoneAsYouTypeFormatter("US");
                slowInitDone = true;
            }).Start();
        }

        PhoneAsYouTypeFormatter _formatter;
        string _locale;

        internal PhoneAsYouTypeFormatter Formatter 
        {
            get 
            {
                if (_formatter == null)
                {
                    _formatter = new PhoneAsYouTypeFormatter(_locale);
                    if (!slowInitDone && NSThread.IsMain)
                    {
                        Console.WriteLine("Warning: Slow PhoneAsYouTypeFormatter init detected. Please call PhoneNumberFormatter.Warm() in background thread to warm formatter cache");
                        slowInitDone = true;
                    }
                }
                return _formatter;
            }
        }

        public PhoneNumberFormatter(string locale = "US")
        {
            _locale = locale;
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
                if (Formatter.Output != _field.Text)
                {
                    changingText = true;
                    _field.Text = Formatter.Reset(_field.Text);
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
            if (Formatter.Output != tf.Text)
            {
                tf.Text = Formatter.Reset(tf.Text);
            }
            else
            {
                var formattedText = tf.Text;
                var newPhoneNumberCursorPos = CountPhoneNumberChars(formattedText, (int)range.Location) + CountPhoneNumberChars(chars, chars.Length);
                formattedText = Formatter.Replace((int)range.Location, (int)range.Length, chars);
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
                if (Formatter.IsPhoneNumberDigit(chars[i]))
                    result++;
            }
            return result;
        }

        private int FormattedTextCursorPosition(string formattedText, int phoneNumberCursorPosition)
        {
            var newCursorOffset = 0;
            while (newCursorOffset < formattedText.Length && phoneNumberCursorPosition > 0)
            {
                if (Formatter.IsPhoneNumberDigit(formattedText[newCursorOffset]))
                {
                    phoneNumberCursorPosition--;
                }
                newCursorOffset++;
            }
            return newCursorOffset;
        }
    }
}
