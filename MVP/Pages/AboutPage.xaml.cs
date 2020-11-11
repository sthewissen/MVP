using MVP.Services.Interfaces;

namespace MVP.Pages
{
    public partial class AboutPage
    {
        public AboutPage(IAnalyticsService analyticsService)
             : base(analyticsService) => InitializeComponent();

        void ScrollView_Scrolled(System.Object sender, Xamarin.Forms.ScrolledEventArgs e)
        {
            appFrame.ShadowOpacity = e.ScrollY / 50 > 1 ? 1 : e.ScrollY / 50;
        }
    }
}
