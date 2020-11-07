using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using MVP.Services.Interfaces;
using System;
using MVP.Services;
using MVP.Pages;
using TinyMvvm.Autofac;
using TinyMvvm.IoC;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.Helpers;

namespace MVP
{
    public partial class App : Xamarin.Forms.Application
    {
        readonly IAnalyticsService analyticsService;
        readonly IAuthService authService;
        readonly IDialogService dialogService;

        public static IMvpApiService MvpApiService { get; set; }

        public App(IAnalyticsService analyticsService,
                    IMvpApiService mvpApiService,
                    IAuthService authService,
                    IDialogService dialogService)
        {
            InitializeComponent();

            this.analyticsService = analyticsService;
            this.authService = authService;
            this.dialogService = dialogService;

            // We add exception handling here, because the MVP API is shared
            // through this app class with every page. Errors in it need to handled
            // generically through here as well.
            mvpApiService.AccessTokenExpired += MvpApiService_AccessTokenExpired;
            mvpApiService.RequestErrorOccurred += MvpApiService_RequestErrorOccurred;

            MvpApiService = mvpApiService;

            Device.SetFlags(new[] { "IndicatorView_Experimental" });
            Resolver.SetResolver(new AutofacResolver(ContainerService.Container));
            Sharpnado.Shades.Initializer.Initialize(loggerEnable: false);
            Akavache.Registrations.Start(Constants.AppName);
            On<iOS>().SetHandleControlUpdatesOnMainThread(true);

            // Set our start page to the splash screen, as that is what we want
            // everyone to see first. It's glorious.
            var navHelper = Resolver.Resolve<INavigationHelper>();
            navHelper.SetRootView(nameof(SplashScreenPage));
        }

        async void MvpApiService_RequestErrorOccurred(object sender, Services.Helpers.ApiServiceEventArgs e)
        {
            if (e.IsBadRequest)
            {
                await dialogService.AlertAsync(
                    "That request wasn't quite right. Try again later.",
                    "Oh boy, that's not good!",
                    "OK");
            }
            else if (e.IsServerError)
            {
                await dialogService.AlertAsync(
                    "The MVP API messed something up. Couldn't grab that data right now.",
                    "Oh boy, that's not good!",
                    "OK");
            }
        }

        async void MvpApiService_AccessTokenExpired(object sender, Services.Helpers.ApiServiceEventArgs e)
        {
            // If the access token expired, we need to sign in again,
            // because we might've lost our auth.
            var result = await authService.SignInAsync();

            if (!result)
            {
                // Show a message that data could not be refreshed. Also forward the user back to getting started
                // telling the user that a logout has occurred.
                await dialogService.AlertAsync(
                    "Your credentials have expired. We needed to log you out. Please login again to continue.",
                    "Oh boy, that's not good!",
                    "OK");

                // Move the user over to Getting Started.
                await authService.SignOutAsync();

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