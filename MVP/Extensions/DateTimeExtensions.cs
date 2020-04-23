using System;
namespace MVP.Extensions
{
    public static class DateTimeExtensions
    {
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
