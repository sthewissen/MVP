using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MVP.Models
{
    public class ContributionArea
    {
        public string AwardName { get; set; }

        // Oddly named collection, but gotta do with what's given to us.
        [JsonProperty(PropertyName = "ContributionArea")]
        public IReadOnlyList<ContributionTechnology> ContributionTechnology { get; set; }
    }
}
