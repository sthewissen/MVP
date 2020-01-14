using System;

namespace MVP.Models
{
    /// <summary>
    /// The activity type.
    /// </summary>
    public partial class ActivityType
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }
}
