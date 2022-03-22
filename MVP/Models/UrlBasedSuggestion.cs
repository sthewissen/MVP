using System;
namespace MVP.Models
{
    public class UrlBasedSuggestion
    {
        public string Url { get; set; }
        public ContributionTechnology ContributionTechnology { get; set; }
        public ContributionType ContributionType { get; set; }
    }
}
