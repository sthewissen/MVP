using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using FormsToolkit;
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
            SignInCommand = new AsyncCommand(() => SignIn());
        }

        async Task SignIn()
        {
            try
            {
                // Pop a sign in request up for the user.
                if (await _authService.SignInAsync().ConfigureAwait(false))
                {
                    MessagingService.Current.SendMessage<bool>(Messaging.AuthorizationComplete, true);
                }
                else
                {
                    MessagingService.Current.SendMessage<bool>(Messaging.AuthorizationComplete, false);
                }
            }
            catch (Exception e)
            {
                e.LogException();

                MessagingService.Current.SendMessage<bool>(Messaging.AuthorizationComplete, false);
            }
        }
    }
}
