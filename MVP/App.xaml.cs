using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using MVP.Services.Interfaces;
using System;
using MVP.Services;
using MVP.Pages;
using TinyMvvm.Autofac;
using MVP.Resources;
using MVP.Services.Demo;
using TinyMvvm;
using Xamarin.CommunityToolkit.Helpers;

namespace MVP
{
    public partial class App : Xamarin.Forms.Application
    {
        public IAnalyticsService AnalyticsService { get; set; }
        public static IMvpApiService MvpApiService { get; set; }
        public static IAuthService AuthService { get; set; }
        public static INavigationHelper NavigationHelper { get; set; }

        public App(IAnalyticsService analyticsService, IMvpApiService mvpApiService, IAuthService authService, LanguageService languageService)
        {
            InitializeComponent();

            this.AnalyticsService = analyticsService;

            // We add exception handling here, because the MVP API is shared
            // through this app class with every page. Errors in it need to handled
            // generically through here as well.
            mvpApiService.AccessTokenExpired += MvpApiService_AccessTokenExpired;

            MvpApiService = mvpApiService;
            AuthService = authService;

            if (Device.RuntimePlatform == Device.Android)
                languageService.Initialize();

            Resolver.SetResolver(new AutofacResolver(ContainerService.Container));
            Akavache.Registrations.Start(Constants.AppName);
            On<iOS>().SetHandleControlUpdatesOnMainThread(true);

            // Set the theme that the user has picked.
            Current.UserAppTheme = Settings.AppTheme;

            // Set our start page to the splash screen, as that is what we want
            // everyone to see first. It's glorious.
            NavigationHelper = Resolver.Resolve<INavigationHelper>();
            NavigationHelper.SetRootView(nameof(SplashScreenPage));
        }

        /// <summary>
        /// Switches from demo to normal mode.
        /// </summary>
        public void SwitchDemoMode(bool enable)
        {
            if (enable)
            {
                MvpApiService = new DemoMvpApiService();
                AuthService = new DemoAuthService();
            }
            else
            {
                MvpApiService = Resolver.Resolve<IMvpApiService>();
                AuthService = Resolver.Resolve<IAuthService>();
            }
        }

        async void MvpApiService_RequestErrorOccurred(object sender, Services.Helpers.ApiServiceEventArgs e)
        {
            if (e.IsBadRequest)
            {
                await DialogService.AlertAsync(Translations.error_badrequest, Translations.error_title, Translations.ok);
            }
            else if (e.IsServerError)
            {
                await DialogService.AlertAsync(Translations.error_servererror, Translations.error_title, Translations.ok);
            }
        }

        async void MvpApiService_AccessTokenExpired(object sender, Services.Helpers.ApiServiceEventArgs e)
        {
            if (e.IsTokenRefreshNeeded)
            {
                // If the access token expired, we need to sign in again,
                // because we might've lost our auth.
                var result = await AuthService.SignInAsync();

                if (!result)
                {
                    // Show a message that data could not be refreshed. Also forward the user back to getting started
                    // telling the user that a logout has occurred.
                    await DialogService.AlertAsync(Translations.alert_error_unauthorized, Translations.error_title, Translations.ok);

                    // Move the user over to Getting Started.
                    await AuthService.SignOutAsync();

                    var navHelper = Resolver.Resolve<INavigationHelper>();
                    navHelper.SetRootView(nameof(IntroPage));
                }
            }
        }

        readonly WeakEventManager resumedEventManager = new WeakEventManager();
        void OnResumed() => resumedEventManager.RaiseEvent(this, System.EventArgs.Empty, nameof(Resumed));

        public event EventHandler Resumed
        {
            add => resumedEventManager.AddEventHandler(value);
            remove => resumedEventManager.RemoveEventHandler(value);
        }

        protected override void OnStart()
        {
            base.OnStart();
            AnalyticsService.Track("App Started");
        }

        protected override void OnResume()
        {
            base.OnResume();

            MvpApiService.AccessTokenExpired += MvpApiService_AccessTokenExpired;

            OnResumed();

            AnalyticsService.Track("App Resumed");
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            MvpApiService.AccessTokenExpired -= MvpApiService_AccessTokenExpired;

            AnalyticsService.Track("App Backgrounded");
        }
    }
}