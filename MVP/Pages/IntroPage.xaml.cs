using FFImageLoading;
using FormsToolkit;
using MVP.Models;
using MVP.PageModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class IntroPage : ContentPage
    {
        public IntroPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            pillbox.Margin = new Thickness(-20, phoneImage.Height / 5, 0, 0);
        }

        void CarouselView_PositionChanged(System.Object sender, Xamarin.Forms.PositionChangedEventArgs e)
        {
            var item = carousel.CurrentItem as OnboardingItem;
            phoneImage.Source = item.ImageName;
        }
    }
}
