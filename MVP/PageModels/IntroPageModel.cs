using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Services;
using MVP.Services.Helpers;
using Xamarin.Essentials;

namespace MVP.PageModels
{
    public class IntroPageModel : BasePageModel
    {
        readonly AuthService _authService;

        public IAsyncCommand SignInCommand { get; set; }

        public IntroPageModel(AuthService authService)
        {
            _authService = authService;
            SignInCommand = new AsyncCommand(SignIn);
        }

        async Task SignIn()
        {
            try
            {
                // Pop a sign in request up for the user.
                if (await _authService.SignInAsync().ConfigureAwait(false))
                {
                    // Init the MVP service.
                    await (App.Current as App).InitializeMvpService();

                    // Set that we've seen the intro.
                    Preferences.Set(Settings.HasSeenIntro, true);

                    // Switch out this page, we don't need it anymore.
                    (App.Current as App).SwitchToRootNavigation();
                }
                else
                {
                    // TODO: Inform the user.
                }
            }
            catch (Exception e)
            {
                e.LogException();
                // TODO: Inform the user.
            }
        }
    }
}
