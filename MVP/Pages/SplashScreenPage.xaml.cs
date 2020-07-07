using System.Diagnostics;
using MVP.Services.Interfaces;
using MVP.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MVP.Pages
{
    /// <summary>
    /// This is the page the user ends up when he/she starts the app.
    /// It checks whether we can silently sign in the user and directs the user to the
    /// correct page based on the result.
    /// </summary>
    public partial class SplashScreenPage
    {
        public SplashScreenPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StartPulseAnimation();
        }

        void StartPulseAnimation()
        {
            var animation = new Animation();

            animation.WithConcurrent((f) => logo.Scale = f, logo.Scale, logo.Scale, Easing.Linear, 0, 0.1);
            animation.WithConcurrent((f) => logo.Scale = f, logo.Scale, logo.Scale * 1.05, Easing.Linear, 0.1, 0.4);
            animation.WithConcurrent((f) => logo.Scale = f, logo.Scale * 1.05, logo.Scale, Easing.Linear, 0.4, 1);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                logo.Animate("Pulse", animation, 16, 1000, repeat: () => true);
            });
        }
    }
}
