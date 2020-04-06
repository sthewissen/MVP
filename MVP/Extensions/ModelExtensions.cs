using System;
using System.Collections.Generic;
using System.Linq;
using MVP.Models;
using MvvmHelpers;

namespace MVP.Extensions
{
    public static class ModelExtensions
    {
        public static IList<Grouping<int, Contribution>> ToGroupedContributions(this ContributionList list)
        {
            var result = new List<Grouping<int, Contribution>>();
            DateTime periodStart = new DateTime(DateTime.Now.Year, 4, 1);

            // If we are before the 1st of April, the period start is last year's.
            if (DateTime.Now < periodStart)
            {
                periodStart = periodStart.AddYears(-1);
            }

            var thisPeriod = list.Contributions.Where(x => x.StartDate >= periodStart).OrderByDescending(x => x.StartDate);
            var lastPeriod = list.Contributions.Where(x => x.StartDate < periodStart).OrderByDescending(x => x.StartDate);

            result.Add(new Grouping<int, Contribution>(0, thisPeriod));
            result.Add(new Grouping<int, Contribution>(1, lastPeriod));

            return result;
        }
    }
}
