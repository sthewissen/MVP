using System.Collections.Generic;
using Newtonsoft.Json;

namespace MVP.Models
{
    public class ContributionCategory
    {
        public string AwardCategory { get; set; }

        // Oddly named collection, but gotta do with what's given to us.
        [JsonProperty(PropertyName = "Contributions")]
        public IReadOnlyList<ContributionArea> ContributionAreas { get; set; }
    }
}