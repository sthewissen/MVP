using MVP.Models;
using MVP.Services.Interfaces;
using MVP.ViewModels;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class IntroPage
    {
        public IntroPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        void CarouselView_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            var item = carousel.CurrentItem as OnboardingItem;
            phoneImage.Source = item.ImageName;
        }
    }
}
