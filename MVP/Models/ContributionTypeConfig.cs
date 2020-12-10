using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.Models
{
    public class ContributionTypeConfig
    {
        public string AnnualQuantityHeader { get; set; }
        public string SecondAnnualQuantityHeader { get; set; }
        public string AnnualReachHeader { get; set; }

        public bool HasAnnualQuantity { get; set; } = true; // This is always there.
        public bool HasSecondAnnualQuantity { get; set; }
        public bool HasAnnualReach { get; set; }

        public string TypeColor { get; set; }
        public string TextColor { get; set; }

        public bool IsUrlRequired { get; set; }
        public bool IsAnnualQuantityRequired { get; set; } = true; // This is always required.
        public bool IsSecondAnnualQuantityRequired { get; set; }
        public bool IsAnnualReachRequired { get; set; }
    }
}
