using System;

namespace MVP.Models
{
    public partial class AwardRecognition
    {
        public int? PrivateSiteId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateEarned { get; set; }
        public string ReferenceUrl { get; set; }
        public Visibility AwardRecognitionVisibility { get; set; }
    }
}
