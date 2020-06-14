using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using FormsToolkit;
using MVP.Services;
using MVP.Services.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.PageModels
{
    public class SplashScreenPageModel : BasePageModel
    {
        readonly IAuthService _authService;
        readonly IMvpApiService _mvpApiService;

        public IAsyncCommand PrefetchDataCommand { get; set; }

        public string FetchText { get; set; }

        public SplashScreenPageModel(IAuthService authService, IMvpApiService mvpApiService)
        {
            _authService = authService;
            _mvpApiService = mvpApiService;

            PrefetchDataCommand = new AsyncCommand(() => PrefetchData());
        }

        async Task PrefetchData()
        {
            try
            {
                if (await _authService.SignInSilentAsync())
                {
                    FetchText = "Grabbing the contribution areas...";
                    await _mvpApiService.GetContributionAreasAsync();

                    FetchText = "Syncing the contribution types...";
                    await _mvpApiService.GetContributionTypesAsync();

                    FetchText = "Putting visibilities in place...";
                    await _mvpApiService.GetVisibilitiesAsync();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var page = FreshMvvm.FreshPageModelResolver.ResolvePageModel<ContributionsPageModel>();
                        var navigation = new FreshMvvm.FreshNavigationContainer(page) { BarTextColor = Color.Black };
                        Application.Current.MainPage = navigation;
                    });
                }
                else
                {
                    // Fixed delay to show the animation on the frontend :$
                    await Task.Delay(2500);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<IntroPageModel>();
                    });
                }
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<IntroPageModel>();
                });
            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            PrefetchDataCommand.Execute(null);
        }
    }
}
