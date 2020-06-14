using Xamarin.Forms;
using MVP.PageModels;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using MVP.Services.Interfaces;
using System;
using AsyncAwaitBestPractices;
using Akavache;

namespace MVP
{
    public partial class App : Xamarin.Forms.Application
    {
        readonly WeakEventManager _resumedEventManager = new WeakEventManager();
        readonly IAnalyticsService _analyticsService;

        public App(IAnalyticsService analyticsService)
        {
            InitializeComponent();

            _analyticsService = analyticsService;

            Device.SetFlags(new[] { "IndicatorView_Experimental" });

            AppContainer.Build();

            MainPage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<SplashScreenPageModel>();

            On<iOS>().SetHandleControlUpdatesOnMainThread(true);
        }

        public event EventHandler Resumed
        {
            add => _resumedEventManager.AddEventHandler(value);
            remove => _resumedEventManager.RemoveEventHandler(value);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _analyticsService.Track("App Started");
        }

        protected override void OnResume()
        {
            base.OnResume();

            OnResumed();

            _analyticsService.Track("App Resumed");
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            _analyticsService.Track("App Backgrounded");
        }

        void OnResumed() => _resumedEventManager.HandleEvent(this, EventArgs.Empty, nameof(Resumed));

        //public void SwitchToRootNavigation()
        //{
        //    var nav = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<ContributionsPageModel>());
        //    MainPage = nav;
        //}

        //protected async override void OnStart()
        //{
        //    base.OnStart();

        //    // If we have internet and you've seen the intro.
        //    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        //    {
        //        // If the intro hasn't been shown yet, the login is handled through there.
        //        if (Preferences.Get(Settings.HasSeenIntro, false))
        //        {
        //            await SignInAsync().ConfigureAwait(false);
        //        }
        //    }
        //    else
        //    {
        //        // TODO: No internet means no signing in :(
        //        MessagingService.Current.SendMessage(Messaging.AppOffline);
        //    }
        //}

        //async Task SignInAsync()
        //{
        //    // Get the token to see if there is one.
        //    var refreshToken = await SecureStorage.GetAsync("AccessToken").ConfigureAwait(false);

        //    // If refresh token is available, the user has previously been logged in and we can get a refreshed access token immediately
        //    if (!string.IsNullOrEmpty(refreshToken))
        //    {
        //        // Try to login without the user's interference.
        //        if (await AuthService.SignInSilentAsync().ConfigureAwait(false))
        //        {
        //            await InitializeMvpService().ConfigureAwait(false);
        //            MessagingService.Current.SendMessage(Messaging.SuccessfulAuth);
        //        }
        //        else
        //        {
        //            // Couldn't log in, have to force the user to provide new credentials.
        //            if (await AuthService.SignInAsync().ConfigureAwait(false))
        //            {
        //                await InitializeMvpService().ConfigureAwait(false);
        //                MessagingService.Current.SendMessage(Messaging.SuccessfulAuth);
        //            }
        //            else
        //            {
        //                // TODO: User wasn't logged in.
        //                MessagingService.Current.SendMessage(Messaging.InvalidAuth);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // No stored credentials, use login workflow
        //        if (await AuthService.SignInAsync().ConfigureAwait(false))
        //        {
        //            await InitializeMvpService().ConfigureAwait(false);
        //            MessagingService.Current.SendMessage(Messaging.SuccessfulAuth);
        //        }
        //        else
        //        {
        //            // TODO: User wasn't logged in.
        //            MessagingService.Current.SendMessage(Messaging.InvalidAuth);
        //        }
        //    }
        //}

        //public async Task InitializeMvpService()
        //{
        //    // Grab the auth token and set it to the MVP service.
        //    var token = await SecureStorage.GetAsync("AccessToken").ConfigureAwait(false);
        //    var service = new MvpApiService(token);

        //    if (MvpApiService != null)
        //    {
        //        MvpApiService.AccessTokenExpired -= MvpApiService_AccessTokenExpired;
        //        MvpApiService.RequestErrorOccurred -= MvpApiService_RequestErrorOccurred;
        //    }

        //    service.AccessTokenExpired += MvpApiService_AccessTokenExpired;
        //    service.RequestErrorOccurred += MvpApiService_RequestErrorOccurred;

        //    MvpApiService = service;
        //}

        //private void MvpApiService_RequestErrorOccurred(object sender, ApiServiceEventArgs e)
        //{
        //    // TODO: Implement generic error handling.
        //}

        //private async void MvpApiService_AccessTokenExpired(object sender, ApiServiceEventArgs e)
        //{
        //    // No valid credentials, use login workflow
        //    await SignInAsync().ConfigureAwait(false);
        //}
    }
}