using System;
using Foundation;

namespace Qoden.UI
{
    public static class DateUtil
    {
        public static DateTime ToDateTime(this NSDate date)
        {
            var reference = TimeZone.CurrentTimeZone.ToLocalTime(
                new DateTime(2001, 1, 1, 0, 0, 0));
            return reference.AddSeconds(date.SecondsSinceReferenceDate);
        }

        public static NSDate ToNSDate(this DateTime date)
        {
            var reference = TimeZone.CurrentTimeZone.ToLocalTime(
                new DateTime(2001, 1, 1, 0, 0, 0));
            return NSDate.FromTimeIntervalSinceReferenceDate(
                (date - reference).TotalSeconds);
        }
    }
}
