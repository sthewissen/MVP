using Xamarin.Forms;
using MVP.PageModels;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using MVP.Services.Interfaces;
using System;
using AsyncAwaitBestPractices;
using Akavache;
using MVP.Services;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace MVP
{
    public partial class App : Xamarin.Forms.Application
    {
        readonly WeakEventManager _resumedEventManager = new WeakEventManager();
        readonly IAnalyticsService _analyticsService;
        readonly IAuthService _authService;
        readonly IDialogService _dialogService;

        public static IMvpApiService MvpApiService { get; set; }

        public App(IAnalyticsService analyticsService, IMvpApiService mvpApiService,
            IAuthService authService, IDialogService dialogService)
        {
            InitializeComponent();

            _analyticsService = analyticsService;
            _authService = authService;
            _dialogService = dialogService;

            mvpApiService.AccessTokenExpired += MvpApiService_AccessTokenExpired;
            mvpApiService.RequestErrorOccurred += MvpApiService_RequestErrorOccurred;

            MvpApiService = mvpApiService;

            Device.SetFlags(new[] { "IndicatorView_Experimental" });

            AppContainer.Build();
            Akavache.Registrations.Start(Constants.AppName);

            MainPage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<SplashScreenPageModel>();

            On<iOS>().SetHandleControlUpdatesOnMainThread(true);
        }

        async void MvpApiService_RequestErrorOccurred(object sender, Services.Helpers.ApiServiceEventArgs e)
        {
            if (e.IsBadRequest)
            {
                await _dialogService.AlertAsync(
                    "That request wasn't quite right. Try again later.",
                    "Oh boy, that's not good!",
                    "OK");
            }
            else if (e.IsServerError)
            {
                await _dialogService.AlertAsync(
                    "The MVP API messed something up. Couldn't grab that data right now.",
                    "Oh boy, that's not good!",
                    "OK");
            }
        }

        async void MvpApiService_AccessTokenExpired(object sender, Services.Helpers.ApiServiceEventArgs e)
        {
            // Log in again.
            var result = await _authService.SignInAsync();

            if (!result)
            {
                // Show a message that data could not be refreshed.
                // Also forward the user back to getting started, telling the user that
                // a logout has occurred.
                await _dialogService.AlertAsync(
                    "Your credentials have expired. We needed to log you out. Please login again to continue.",
                    "Oh boy, that's not good!",
                    "OK");
            }
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