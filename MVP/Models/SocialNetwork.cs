using System;

namespace MVP.Models
{
    public partial class SocialNetwork
    {
        public Guid? SocialNetworkId { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public SocialNetworkStatusCode StatusCode { get; set; }
        public bool? SystemCollectionEnabled { get; set; }
    }
}
