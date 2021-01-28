using System;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Helpers;
using MVP.Pages;
using MVP.Services.Interfaces;
using Plugin.StoreReview;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class SplashScreenViewModel : BaseViewModel
    {
        string fetchText = " ";

        public IAsyncCommand PrefetchDataCommand { get; }

        public string FetchText
        {
            get => fetchText;
            set => Set(ref fetchText, value);
        }

        public SplashScreenViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            PrefetchDataCommand = new AsyncCommand(() => PrefetchData());
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            PrefetchData().SafeFireAndForget();
        }

        /// <summary>
        /// Prefetches some data that can come in handy (lookups).
        /// </summary>
        async Task PrefetchData()
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    await GoToNextPage(false);

                if (await AuthService.SignInSilentAsync())
                {
                    FetchText = Resources.Translations.splash_contributionareas;
                    await MvpApiService.GetContributionAreasAsync();

                    FetchText = Resources.Translations.splash_contributiontypes;
                    await MvpApiService.GetContributionTypesAsync();

                    FetchText = Resources.Translations.splash_visibilities;
                    await MvpApiService.GetVisibilitiesAsync();

                    await GoToNextPage(true);
                }
                else
                {
                    // Fixed delay to show the animation on the frontend :$
                    FetchText = Resources.Translations.splash_gettingready;
                    await Task.Delay(2500);
                    await GoToNextPage(false);
                }
            }
            catch (Exception e)
            {
                AnalyticsService.Report(e);
                await GoToNextPage(false);
            }
        }

        /// <summary>
        /// Depending on whether we're authenticated or not we show a specific page.
        /// </summary>
        public Task GoToNextPage(bool isAuthenticated)
            => MainThread.InvokeOnMainThreadAsync(()
                =>
            {
                if (isAuthenticated)
                {
                    AnalyticsService.Track("Splash Authenticated");
                    NavigationHelper.SetRootView(nameof(TabbedMainPage), false);

#if !DEBUG
                    if (!Settings.IsUsingDemoAccount)
                    {
                        var count = Settings.StartupCount;
                        count++;

                        if (count == 5)
                        {
                            CrossStoreReview.Current.RequestReview();
                        }

                        Settings.StartupCount = count;
                    }
#endif
                }
                else
                {
                    AnalyticsService.Track("Splash Unauthenticated");
                    NavigationHelper.SetRootView(nameof(IntroPage));
                }
            });
    }
}
