using System;

namespace MVP.Models
{
    public partial class OnlineIdentity
    {
        public Guid? OnlineIdentityId { get; set; }
        public Guid? MvpGuid { get; set; }
        public string Name { get; set; }
        public int? PrivateSiteId { get; set; }
        public SharingPreference OnlineIdentityVisibility { get; set; }
        public SocialNetwork SocialNetwork { get; set; }
        public string Url { get; set; }
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public string MicrosoftAccount { get; set; }
        public bool? ContributionCollected { get; set; }
        public bool? PrivacyConsentStatus { get; set; }
        public bool? PrivacyConsentCheckStatus { get; set; }
        public DateTime? PrivacyConsentCheckDate { get; set; }
        public DateTime? PrivacyConsentUnCheckDate { get; set; }
        public bool? Submitted { get; set; }
    }
}
