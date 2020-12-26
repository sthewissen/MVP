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

        public string Initials
        {
            get
            {
                if (string.IsNullOrEmpty(FullName))
                    return "?";

                var parts = FullName.Split(" ");

                if (parts.Length == 0)
                    return "?";
                if (parts.Length == 1)
                    return parts[0].Substring(0, 1).ToUpper();
                else
                    return $"{parts[0].Substring(0, 1).ToUpper()}{parts[1].Substring(0, 1).ToUpper()}";
            }
        }
    }
}
