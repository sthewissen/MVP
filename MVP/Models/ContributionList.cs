using System.Collections.Generic;

namespace MVP.Models
{
    public partial class ContributionList
    {
        public IList<Contribution> Contributions { get; set; }
        public int? TotalContributions { get; set; }
        public int? PagingIndex { get; set; }
    }
}
