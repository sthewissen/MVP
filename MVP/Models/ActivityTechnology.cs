using System;

namespace MVP.Models
{
    /// <summary>
    /// The activity technology model.
    /// </summary>
    public partial class ActivityTechnology
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AwardName { get; set; }
        public string AwardCategory { get; set; }
        public int? Statuscode { get; set; }
        public bool? Active { get; set; }
    }
}
