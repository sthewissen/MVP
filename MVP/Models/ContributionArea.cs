using System.Collections.Generic;
using Newtonsoft.Json;

namespace MVP.Models
{
    public class ContributionAreaWrapper
    {
        public string AwardName { get; set; }

        // Oddly named collection, but gotta do with what's given to us.
        public IReadOnlyList<ContributionTechnology> ContributionArea { get; set; }
    }
}
