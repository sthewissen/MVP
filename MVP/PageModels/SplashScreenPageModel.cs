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
        readonly AuthService _authService;

        public IAsyncCommand CheckAuthorizationCommand { get; set; }

        public SplashScreenPageModel(AuthService authService)
        {
            _authService = authService;
            CheckAuthorizationCommand = new AsyncCommand(() => CheckAuthorization());
        }

        async Task CheckAuthorization()
        {
            try
            {
                if (await _authService.SignInSilentAsync())
                {
                    // Fixed delay to show the animation on the frontend :$
                    await Task.Delay(2500);

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
            CheckAuthorizationCommand.Execute(null);
        }
    }
}
