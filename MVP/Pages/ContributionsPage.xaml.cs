using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class ContributionsPage : ContentPage
    {
        public bool _fabIsOutOfView;

        public ContributionsPage()
        {
            InitializeComponent();
        }

        async void CollectionView_Scrolled(System.Object sender, Xamarin.Forms.ItemsViewScrolledEventArgs e)
        {
            if (!_fabIsOutOfView && e.VerticalOffset > 0)
            {
                await fab.TranslateTo(0, 100, 250, Easing.CubicInOut).ConfigureAwait(false);
                _fabIsOutOfView = true;
            }
            else if (_fabIsOutOfView && e.VerticalOffset < 40)
            {
                await fab.TranslateTo(0, 0, 250, Easing.CubicInOut).ConfigureAwait(false);
                _fabIsOutOfView = false;
            }
        }
    }
}