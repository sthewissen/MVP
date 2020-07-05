using MVP.Models;
using MVP.Services.Interfaces;
using MVP.ViewModels;
using Xamarin.Forms;

namespace MVP.Pages
{
    /// <summary>
    /// This page contains the carousel used for onboarding the user to the app
    /// and the features it offers. 
    /// </summary>
    public partial class IntroPage
    {
        public IntroPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        protected override void OnAppearing()
        {
            base.OnAppearing();
            pillbox.Margin = new Thickness(-20, phoneImage.Height / 5, 0, 0);
        }

        void CarouselView_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            var item = carousel.CurrentItem as OnboardingItem;
            phoneImage.Source = item.ImageName;
        }
    }
}
