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

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            CheckAuthorizationCommand.Execute(null);
        }
    }
}
