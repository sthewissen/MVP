using MVP.Services.Interfaces;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class ContributionTechnologyPickerPage
    {
        public ContributionTechnologyPickerPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        public void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
            => shadow.Opacity = e.VerticalOffset / 50 > 1 ? 1 : e.VerticalOffset / 50;
    }
}
