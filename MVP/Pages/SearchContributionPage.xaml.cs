using MVP.Services.Interfaces;

namespace MVP.Pages
{
    public partial class SearchContributionPage
    {
        public SearchContributionPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();
    }
}
