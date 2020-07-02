using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.PageModels
{
    public class SplashScreenPageModel : BasePageModel
    {
        readonly IAuthService _authService;

        public IAsyncCommand PrefetchDataCommand { get; set; }
        public string FetchText { get; set; }

        public SplashScreenPageModel(IAuthService authService)
        {
            _authService = authService;
            PrefetchDataCommand = new AsyncCommand(() => PrefetchData());
        }

        async Task PrefetchData()
        {
            try
            {
                if (await _authService.SignInSilentAsync())
                {
                    FetchText = "Grabbing the contribution areas...";
                    await MvpApiService.GetContributionAreasAsync();

                    FetchText = "Syncing the contribution types...";
                    await MvpApiService.GetContributionTypesAsync();

                    FetchText = "Putting visibilities in place...";
                    await MvpApiService.GetVisibilitiesAsync();

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
                    FetchText = "Getting things ready...";
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
                    // TODO: Inform the user of this.

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
