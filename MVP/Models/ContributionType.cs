using System;

namespace MVP.Models
{
    public partial class ContributionType
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }
}