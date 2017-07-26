using System;
namespace Qoden.UI
{
    public class PhoneAsYouTypeFormatter
    {
        PhoneNumbers.AsYouTypeFormatter formatter;
        string phoneNumber = string.Empty;

        public PhoneAsYouTypeFormatter(string region)
        {
            formatter = new PhoneNumbers.AsYouTypeFormatter(region);
        }

        public void Clear()
        {
            formatter.Clear();
        }

        public string InputDigit(char c, bool filter = true)
        {
            if (filter && !IsPhoneNumberDigit(c))
            {
                return phoneNumber;
            }
            return phoneNumber = formatter.InputDigit(c);
        }

        public bool IsPhoneNumberDigit(char c)
        {
            return (c >= '0' && c <= '9') || c == '+';
        }

        public string InputDigits(string value, bool filter = true)
        {
            foreach (var c in value)
            {
                InputDigit(c, filter);
            }
            return Output;
        }

        public string Remove(int idx, int count = 1)
        {
            Reset(phoneNumber.Remove(idx, count));
            return phoneNumber;
        }

        public string Replace(int idx, int count, string replacement)
        {
            var str = phoneNumber.Insert(idx, replacement).Remove(idx + replacement.Length, count);
            Reset(str);
            return phoneNumber;
        }

        public string Reset(string value, bool filter = true)
        {
            formatter.Clear();
            phoneNumber = string.Empty;
            InputDigits(value, filter);
            return Output;
        }

        public string Output { get { return phoneNumber; } }
    }
}
