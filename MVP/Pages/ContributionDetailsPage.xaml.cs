using MVP.Services.Interfaces;

namespace MVP.Pages
{
    public partial class ContributionDetailsPage
    {
        public ContributionDetailsPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        void ScrollView_Scrolled(object sender, Xamarin.Forms.ScrolledEventArgs e)
            => appFrame.ShadowOpacity = e.ScrollY / 50 > 1 ? 1 : e.ScrollY / 50;
    }
}
