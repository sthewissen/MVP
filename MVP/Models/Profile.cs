using System.Collections.Generic;

namespace MVP.Models
{
    public partial class Profile
    {
        public string MvpId { get; set; }
        public int? YearsAsMvp { get; set; }
        public string FirstAwardYear { get; set; }
        public string AwardCategoryDisplay { get; set; }
        public string TechnicalExpertise { get; set; }
        public bool? InTheSpotlight { get; set; }
        public string Headline { get; set; }
        public string Biography { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string PrimaryEmailAddress { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingStateCity { get; set; }
        public string Languages { get; set; }
    }
}
