using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using MVP.Services.Interfaces;
using System;
using MVP.Services;
using MVP.Pages;
using TinyMvvm.Autofac;
using TinyMvvm.IoC;
using TinyMvvm;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;
using MVP.Resources;
using MVP.Services.Demo;
using TinyNavigationHelper;

namespace MVP
{
    public partial class App : Xamarin.Forms.Application
    {
        readonly IAnalyticsService analyticsService;

        public static IMvpApiService MvpApiService { get; set; }
        public static IAuthService AuthService { get; set; }

        public App(IAnalyticsService analyticsService, IMvpApiService mvpApiService, IAuthService authService)
        {
            InitializeComponent();

            this.analyticsService = analyticsService;

            // We add exception handling here, because the MVP API is shared
            // through this app class with every page. Errors in it need to handled
            // generically through here as well.
            mvpApiService.AccessTokenExpired += MvpApiService_AccessTokenExpired;
            mvpApiService.RequestErrorOccurred += MvpApiService_RequestErrorOccurred;

            MvpApiService = mvpApiService;
            AuthService = authService;

            LocalizationResourceManager.Current.Init(Translations.ResourceManager);

            Resolver.SetResolver(new AutofacResolver(ContainerService.Container));
            Akavache.Registrations.Start(Constants.AppName);
            On<iOS>().SetHandleControlUpdatesOnMainThread(true);

            // Set the theme that the user has picked.
            Current.UserAppTheme = (OSAppTheme)Preferences.Get(Settings.AppTheme, Settings.AppThemeDefault);

            // Set our start page to the splash screen, as that is what we want
            // everyone to see first. It's glorious.
            var navHelper = Resolver.Resolve<INavigationHelper>();
            navHelper.SetRootView(nameof(SplashScreenPage));
        }

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
                await DialogService.AlertAsync(
                    Translations.alert_error_badrequest,
                    Translations.alert_error_title,
                    Translations.alert_ok);
            }
            else if (e.IsServerError)
            {
                await DialogService.AlertAsync(
                    Translations.alert_error_servererror,
                    Translations.alert_error_title,
                    Translations.alert_ok);
            }
        }

        async void MvpApiService_AccessTokenExpired(object sender, Services.Helpers.ApiServiceEventArgs e)
        {
            // If the access token expired, we need to sign in again,
            // because we might've lost our auth.
            var result = await AuthService.SignInAsync();

            if (!result)
            {
                // Show a message that data could not be refreshed. Also forward the user back to getting started
                // telling the user that a logout has occurred.
                await DialogService.AlertAsync(
                    Translations.alert_error_unauthorized,
                    Translations.alert_error_title,
                    Translations.alert_ok);

                // Move the user over to Getting Started.
                await AuthService.SignOutAsync();

                var navHelper = Resolver.Resolve<INavigationHelper>();
                navHelper.SetRootView(nameof(IntroPage));
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
            analyticsService.Track("App Started");
        }

        protected override void OnResume()
        {
            base.OnResume();

            MvpApiService.AccessTokenExpired += MvpApiService_AccessTokenExpired;
            MvpApiService.RequestErrorOccurred += MvpApiService_RequestErrorOccurred;

            OnResumed();

            analyticsService.Track("App Resumed");
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            MvpApiService.AccessTokenExpired -= MvpApiService_AccessTokenExpired;
            MvpApiService.RequestErrorOccurred -= MvpApiService_RequestErrorOccurred;

            analyticsService.Track("App Backgrounded");
        }
    }
}