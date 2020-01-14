using System;

namespace MVP.Models
{
    public partial class ContributionTechnology
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string AwardName { get; set; }
        public string AwardCategory { get; set; }
        public int? Statuscode { get; set; }
        public bool? Active { get; set; }
    }
}