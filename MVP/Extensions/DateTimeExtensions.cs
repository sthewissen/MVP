using System;

namespace MVP.Extensions
{
    public static class DateTimeExtensions
    {
        // The API doesn't return any data regarding the current initial award date.
        // Someone who became an MVP in October e.g. is an MVP until April + 1 year.
        // That entire period is valid as a period to edit activities in.

        /// <summary>
        /// Retrieves a DateTime representing the start date for the current award period.
        /// </summary>
        public static DateTime CurrentAwardPeriodStartDate(this DateTime dateTime)
        {
            var currentYearStart = new DateTime(DateTime.Now.Year, 4, 1, 0, 0, 0);
            var lastYearStart = new DateTime(DateTime.Now.Year - 1, 4, 1, 0, 0, 0);

            return dateTime.Date >= currentYearStart.Date ? currentYearStart : lastYearStart;
        }

        /// <summary>
        /// Checks whether or not a provided DateTime is within the current award period.
        /// </summary>
        public static bool IsWithinCurrentAwardPeriod(this DateTime dateTime)
        {
            var periodStart = DateTime.UtcNow.CurrentAwardPeriodStartDate();
            return dateTime.Date >= periodStart.Date;
        }

        /// <summary>
        /// Checks whether or not a provided DateTme is within the current award period.
        /// </summary>
        public static bool IsWithinCurrentAwardPeriod(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return false;

            var periodStart = DateTime.UtcNow.CurrentAwardPeriodStartDate();

            return dateTime.Value.Date >= periodStart.Date;
        }
    }
}
