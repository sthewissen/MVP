using MVP.Services.Interfaces;

namespace MVP.Pages
{
    public partial class BadgesPage
    {
        public BadgesPage(IAnalyticsService analyticsService)
             : base(analyticsService) => InitializeComponent();
    }
}
