using MVP.Services.Interfaces;

namespace MVP.Pages
{
    public partial class StatisticsPage
    {
        public StatisticsPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();
    }
}
