using MVP.Services.Interfaces;

namespace MVP.Pages
{
    public partial class ThemePickerPage
    {
        public ThemePickerPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        void CollectionView_Scrolled(object sender, Xamarin.Forms.ItemsViewScrolledEventArgs e)
        {
            appFrame.ShadowOpacity = e.VerticalOffset / 50 > 1 ? 1 : e.VerticalOffset / 50;
        }
    }
}
