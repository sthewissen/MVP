using System;

namespace MVP.Models
{
    /// <summary>
    /// The account certification view model for edit.
    /// </summary>
    public partial class Certification
    {
        public int? PrivateSiteId { get; set; }
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public Visibility CertificationVisibility { get; set; }
    }
}
