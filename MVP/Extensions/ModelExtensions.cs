using System;
using System.Collections.Generic;
using System.Linq;
using MVP.Models;
using MvvmHelpers;

namespace MVP.Extensions
{
    public static class ModelExtensions
    {
        public static IList<Grouping<int, Contribution>> ToGroupedContributions(this IList<Contribution> list)
        {
            var result = new List<Grouping<int, Contribution>>();
            DateTime periodStart = new DateTime(DateTime.Now.Year, 4, 1);

            // If we are before the 1st of April, the period start is last year's.
            if (DateTime.Now < periodStart)
            {
                periodStart = periodStart.AddYears(-1);
            }

            var thisPeriod = list.Where(x => x.StartDate >= periodStart).OrderByDescending(x => x.StartDate);
            var lastPeriod = list.Where(x => x.StartDate < periodStart).OrderByDescending(x => x.StartDate);

            if (thisPeriod.Any())
                result.Add(new Grouping<int, Contribution>(0, thisPeriod));

            if (lastPeriod.Any())
                result.Add(new Grouping<int, Contribution>(1, lastPeriod));

            return result;
        }
    }
}
