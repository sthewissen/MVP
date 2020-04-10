using System;
using System.Threading.Tasks;
using FormsToolkit;
using MVP.PageModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.Pages
{
    /// <summary>
    /// This is the page the user ends up when he/she starts the app.
    /// It checks whether we can silently sign in the user and directs the user to the
    /// correct page based on the result.
    /// </summary>
    public partial class SplashScreenPage : ContentPage
    {
        public SplashScreenPage()
        {
            InitializeComponent();
            MessagingService.Current.Subscribe<bool>(Messaging.AuthorizationComplete, (service, success) => HandleAuthorizationComplete(success));
        }

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

            Device.BeginInvokeOnMainThread(() =>
            {
                logo.Animate("Pulse", animation, 16, 1000, repeat: () => true);
            });
        }

        async void HandleAuthorizationComplete(bool isAuthorizationSuccessful)
        {
            await MainThread.InvokeOnMainThreadAsync(AnimateTransition);

            // Depending on what the result of the silent sign in is we can
            // show the intro or the main screen of the app.
            if (!isAuthorizationSuccessful)
            {
                NavigateToIntroPage();
            }
            else
            {
                NavigateToContributionsPage();
            }
        }

        async Task AnimateTransition()
        {
            // To at least show the pulse animation. Give a feeling that we're loading the app.
            await Task.Delay(2500);

            // Explode the logo and fade to white, which is what the incoming page comes up as.
            var explodeImageTask = Task.WhenAll(Content.ScaleTo(100, 500, Easing.CubicOut), Content.FadeTo(0, 250, Easing.CubicInOut));
            BackgroundColor = Color.White;
            await explodeImageTask;
        }

        void NavigateToContributionsPage()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var page = FreshMvvm.FreshPageModelResolver.ResolvePageModel<ContributionsPageModel>();
                var navigation = new FreshMvvm.FreshNavigationContainer(page);
                Application.Current.MainPage = navigation;
            });
        }

        void NavigateToIntroPage()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<IntroPageModel>();
            });
        }
    }
}
