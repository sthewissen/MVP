using System;
namespace MVP.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime CurrentAwardPeriodStartDate(this DateTime dateTime)
        {
            var currentYearStart = new DateTime(DateTime.Now.Year, 4, 1);
            var lastYearStart = new DateTime(DateTime.Now.Year - 1, 4, 1);

            return dateTime >= currentYearStart ? currentYearStart : lastYearStart;
        }

        public static bool IsWithinCurrentAwardPeriod(this DateTime dateTime)
        {
            var periodStart = new DateTime(DateTime.Now.Year, 4, 1);

            return dateTime >= periodStart;
        }

        public static bool IsWithinCurrentAwardPeriod(this DateTime? dateTime)
        {
            var periodStart = new DateTime(DateTime.Now.Year, 4, 1);

            return dateTime.HasValue && dateTime >= periodStart;
        }
    }
}
