using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using TinyNavigationHelper.Abstraction;
using TinyNavigationHelper.Forms;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class SplashScreenViewModel : BaseViewModel
    {
        string fetchText = string.Empty;

        public IAsyncCommand PrefetchDataCommand { get; }

        public string FetchText
        {
            get => fetchText;
            set => Set(ref fetchText, value);
        }

        public SplashScreenViewModel(IAuthService authService, IAnalyticsService analyticsService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            PrefetchDataCommand = new AsyncCommand(() => PrefetchData());
        }

        async Task PrefetchData()
        {
            try
            {
                if (await AuthService.SignInSilentAsync())
                {
                    FetchText = "Grabbing the contribution areas...";
                    await MvpApiService.GetContributionAreasAsync();

                    FetchText = "Syncing the contribution types...";
                    await MvpApiService.GetContributionTypesAsync();

                    FetchText = "Putting visibilities in place...";
                    await MvpApiService.GetVisibilitiesAsync();

                    await GoToNextPage(true);
                }
                else
                {
                    // Fixed delay to show the animation on the frontend :$
                    FetchText = "Getting things ready...";
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

        public Task GoToNextPage(bool isAuthenticated)
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (isAuthenticated)
                {
                    AnalyticsService.Track("");
                    NavigationHelper.SetRootView(nameof(TabbedMainPage), false);
                }
                else
                {
                    AnalyticsService.Track("");
                    NavigationHelper.SetRootView(nameof(IntroPage));
                }
            });
        }

        public override Task OnAppearing()
        {
            PrefetchDataCommand.Execute(null);
            return base.OnAppearing();
        }
    }
}
