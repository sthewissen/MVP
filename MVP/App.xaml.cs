using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MVP.Services;
using Xamarin.Essentials;
using System.Threading.Tasks;
using MVP.PageModels;
using FreshMvvm;
using MVP.Services.Helpers;
using FormsToolkit;

namespace MVP
{
    public partial class App : Application
    {
        public static MvpApiService MvpApiService { get; set; }
        public static AuthService AuthService { get; set; }

        public App()
        {
            InitializeComponent();

            // Device.SetFlags(new[] { });

            AuthService = new AuthService();

            if (VersionTracking.IsFirstLaunchEver || !Preferences.Get(Settings.HasSeenIntro, false))
            {
                // Upon first launch, show the intro!
                MainPage = FreshPageModelResolver.ResolvePageModel<IntroPageModel>();
            }
            else
            {
                // By default show the main screen, we will update accordingly.
                SwitchToRootNavigation();
            }
        }

        public void SwitchToRootNavigation()
        {
            var nav = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<ContributionsPageModel>(), "MainNavigation");
            MainPage = nav;
        }

        protected async override void OnStart()
        {
            base.OnStart();

            // If we have internet and you've seen the intro.
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                // If the intro hasn't been shown yet, the login is handled through there.
                if (Preferences.Get(Settings.HasSeenIntro, false))
                {
                    await SignInAsync().ConfigureAwait(false);
                }
            }
            else
            {
                // TODO: No internet means no signing in :(
                MessagingService.Current.SendMessage(Messaging.AppOffline);
            }
        }

        async Task SignInAsync()
        {
            // Get the token to see if there is one.
            var refreshToken = await SecureStorage.GetAsync("AccessToken").ConfigureAwait(false);

            // If refresh token is available, the user has previously been logged in and we can get a refreshed access token immediately
            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Try to login without the user's interference.
                if (await AuthService.SignInSilentAsync().ConfigureAwait(false))
                {
                    await InitializeMvpService().ConfigureAwait(false);
                }
                else
                {
                    // Couldn't log in, have to force the user to provide new credentials.
                    if (await AuthService.SignInAsync().ConfigureAwait(false))
                    {
                        await InitializeMvpService().ConfigureAwait(false);
                    }
                    else
                    {
                        // TODO: User wasn't logged in.
                        MessagingService.Current.SendMessage(Messaging.InvalidAuth);
                    }
                }
            }
            else
            {
                // No stored credentials, use login workflow
                if (await AuthService.SignInAsync().ConfigureAwait(false))
                {
                    await InitializeMvpService().ConfigureAwait(false);
                }
                else
                {
                    // TODO: User wasn't logged in.
                    MessagingService.Current.SendMessage(Messaging.InvalidAuth);
                }
            }
        }

        public async Task InitializeMvpService()
        {
            // Grab the auth token and set it to the MVP service.
            var token = await SecureStorage.GetAsync("AccessToken").ConfigureAwait(false);
            var service = new MvpApiService(token);

            if (MvpApiService != null)
            {
                MvpApiService.AccessTokenExpired -= MvpApiService_AccessTokenExpired;
                MvpApiService.RequestErrorOccurred -= MvpApiService_RequestErrorOccurred;
            }

            service.AccessTokenExpired += MvpApiService_AccessTokenExpired;
            service.RequestErrorOccurred += MvpApiService_RequestErrorOccurred;

            MvpApiService = service;
        }

        private void MvpApiService_RequestErrorOccurred(object sender, ApiServiceEventArgs e)
        {
            // TODO: Implement generic error handling.
        }

        private async void MvpApiService_AccessTokenExpired(object sender, ApiServiceEventArgs e)
        {
            // No valid credentials, use login workflow
            await SignInAsync().ConfigureAwait(false);
        }
    }
}