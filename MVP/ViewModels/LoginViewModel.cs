using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Pages;
using MVP.Services;
using MVP.Services.Interfaces;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public IAsyncCommand<WebNavigatedEventArgs> NavigationOccurredCommand { get; set; }
        public Uri WebSource { get; set; }

        public LoginViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            NavigationOccurredCommand = new AsyncCommand<WebNavigatedEventArgs>((x) => NavigationOccurred(x));
            WebSource = Constants.SignInUrl;
        }

        async Task NavigationOccurred(WebNavigatedEventArgs e)
        {
            switch (e.Result)
            {
                case WebNavigationResult.Success:
                    if (e.Url.Contains("code="))
                    {
                        var myUri = new Uri(e.Url);
                        var authCode = myUri.ExtractQueryValue("code");
                        var token = await App.AuthService.RequestAuthorizationAsync(authCode);

                        if (string.IsNullOrEmpty(token))
                        {
                            // Not authed
                            AnalyticsService.Track("Invalid MVP Account Used");
                            await DialogService.AlertAsync(Resources.Translations.error_nomvpaccount, Resources.Translations.error_title, Resources.Translations.ok);
                        }
                        else
                        {
                            // Authed
                            await BackAsync();

                            AnalyticsService.Track("User Logged In");

                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                NavigationHelper.SetRootView(nameof(TabbedMainPage), false);
                            });
                        }
                    }
                    else if (e.Url.Contains("lc="))
                    {
                        // Redirect to signin page if there's a bounce
                        WebSource = Constants.SignInUrl;
                    }
                    break;
                case WebNavigationResult.Failure:
                    break;
                case WebNavigationResult.Timeout:
                    break;
                case WebNavigationResult.Cancel:
                    break;
                default:
                    break;
            }
        }

        // Pop the entire modal stack instead of just going back one screen.
        // This means it's editing mode and there is no way to go back and change activity type.
        public async override Task BackAsync()
        {
            if (Device.RuntimePlatform == Device.iOS)
                await CloseModalAsync().ConfigureAwait(false);
            else
                await base.BackAsync().ConfigureAwait(false);
        }
    }
}
