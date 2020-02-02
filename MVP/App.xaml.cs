using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MVP.Services;
using MVP.Views;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace MVP
{
    public partial class App : Application
    {
        readonly AuthService _authService;
        bool _isIntroAlreadyShown;

        public App()
        {
            InitializeComponent();

            // TODO: Inject/resolve?
            _authService = new AuthService();

            // By default show the main screen, we will update accordingly.
            MainPage = new AppShell();

            // Ideally we also check here for a token in SecureStorage to set the correct
            // initial page of the app. If no token exists, we HAVE to log in anyway.
            _authService.SignInSilentAsync().ContinueWith(t =>
            {
                if (!t.IsFaulted && t.Result)
                {
                    // We could auth, so show our main screen.
                    MainPage = new AppShell();
                }
                else
                {
                    MainPage = new IntroPage();
                    _isIntroAlreadyShown = true;
                }
            });
        }

        protected async override void OnStart()
        {
            await VerifyAuthentication();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected async override void OnResume()
        {
            // Handle when your app resumes
            await VerifyAuthentication();
        }

        async Task VerifyAuthentication()
        {
            // If the intro is shown already we can skip this check.
            if (!_isIntroAlreadyShown)
            {
                var isAuthenticated = await _authService.SignInSilentAsync();

                if (isAuthenticated)
                {
                    MainPage = new AppShell();
                }
                else
                {
                    // Something went wrong using refresh token, so show the intro + interactive login.
                    MainPage = new IntroPage();
                }
            }
        }
    }
}