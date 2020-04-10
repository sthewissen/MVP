using FormsToolkit;
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

            MessagingService.Current.Subscribe<bool>(Messaging.AuthorizationComplete, (service, success) => HandleAuthorizationComplete(success));
        }

        void HandleAuthorizationComplete(bool isAuthorizationSuccessful)
        {
            // Depending on what the result of the silent sign in is we can show the intro or
            // the main screen of the app.
            if (isAuthorizationSuccessful)
            {
                // TODO: SHOW MESSAGE
            }
            else
            {
                NavigateToContributionsPage();
            }
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
    }
}
