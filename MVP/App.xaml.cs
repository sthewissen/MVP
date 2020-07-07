using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using MVP.Services.Interfaces;
using System;
using AsyncAwaitBestPractices;
using MVP.Services;
using MVP.Pages;
using MVP.ViewModels;
using TinyMvvm.Autofac;
using TinyMvvm.IoC;

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

            // Add handling of errors coming from the MVP API.
            mvpApiService.AccessTokenExpired += MvpApiService_AccessTokenExpired;
            mvpApiService.RequestErrorOccurred += MvpApiService_RequestErrorOccurred;
            MvpApiService = mvpApiService;

            Device.SetFlags(new[] { "IndicatorView_Experimental", "Shapes_Experimental" });

            // Initialize TinyMvvm
            Resolver.SetResolver(new AutofacResolver(ContainerService.Container));
            TinyMvvm.Forms.TinyMvvm.Initialize();

            // Initialize Akavache.
            Akavache.Registrations.Start(Constants.AppName);

            // Set our start page.
            MainPage = new SplashScreenPage(analyticsService);

            On<iOS>().SetHandleControlUpdatesOnMainThread(true);
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
            // Log in again.
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
                //MainPage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<IntroViewModel>();
            }
        }

        readonly WeakEventManager resumedEventManager = new WeakEventManager();
        void OnResumed() => resumedEventManager.HandleEvent(this, System.EventArgs.Empty, nameof(Resumed));

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