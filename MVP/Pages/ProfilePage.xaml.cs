using MVP.Services.Interfaces;

namespace MVP.Pages
{
    public partial class ProfilePage
    {
        public ProfilePage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();
    }
}
