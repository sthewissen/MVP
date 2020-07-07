using MVP.Services.Interfaces;

namespace MVP.Pages
{
    public partial class ContributionDetailsPage
    {
        public ContributionDetailsPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();
    }
}
