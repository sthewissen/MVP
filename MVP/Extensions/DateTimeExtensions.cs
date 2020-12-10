using System;

namespace MVP.Extensions
{
    public static class DateTimeExtensions
    {
        // The API doesn't return any data regarding the current initial award date.
        // Someone who became an MVP in October e.g. is an MVP until April + 1 year.
        // That entire period is valid as a period to edit activities in.
        public static DateTime CurrentAwardPeriodStartDate(this DateTime dateTime)
        {
            var currentYearStart = new DateTime(DateTime.Now.Year, 4, 1);
            var lastYearStart = new DateTime(DateTime.Now.Year - 1, 4, 1);

            return dateTime >= currentYearStart ? currentYearStart : lastYearStart;
        }

        public static bool IsWithinReviewWindow(this DateTime dateTime)
        {
            var windowStart = new DateTime(dateTime.Year, 4, 1);
            var windowEnd = new DateTime(dateTime.Year, 6, 1);

            return dateTime >= windowStart && dateTime < windowEnd;
        }

        public static bool IsWithinCurrentAwardPeriod(this DateTime dateTime)
        {
            var periodStart = new DateTime(DateTime.Now.Year, 4, 1);

            return dateTime >= periodStart;
        }

        public static bool IsWithinCurrentAwardPeriod(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return false;

            var periodStart = new DateTime(DateTime.Now.Year, 4, 1);

            return dateTime >= periodStart;
        }
    }
}
