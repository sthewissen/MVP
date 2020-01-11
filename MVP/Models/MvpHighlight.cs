using System;

namespace MVP.Models
{
    public partial class MvpHighlight
    {
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string DateFormatted { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
    }
}
