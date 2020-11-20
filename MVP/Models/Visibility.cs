namespace MVP.Models
{
    /// <summary>
    /// The visibility.
    /// </summary>
    public partial class Visibility
    {
        public int? Id { get; set; }
        public string Description { get; set; }
        public string LocalizeKey { get; set; }

        public override string ToString() => Description;
    }
}
