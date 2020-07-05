using MVP.Services.Interfaces;
using MVP.ViewModels;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class ContributionsPage
    {
        public bool _fabIsOutOfView;

        public ContributionsPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        async void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            //if (!_fabIsOutOfView && e.VerticalOffset > 0)
            //{
            //    await fab.TranslateTo(0, 100, 250, Easing.CubicInOut).ConfigureAwait(false);
            //    _fabIsOutOfView = true;
            //}
            //else if (_fabIsOutOfView && e.VerticalOffset < 40)
            //{
            //    await fab.TranslateTo(0, 0, 250, Easing.CubicInOut).ConfigureAwait(false);
            //    _fabIsOutOfView = false;
            //}
        }
    }
}