using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using FreshMvvm;
using MVP.Services;
using Xamarin.Forms;

namespace MVP.PageModels
{
    public class ProfilePageModel : BasePageModel
    {
        readonly AuthService _authService;

        public IAsyncCommand LogoutCommand { get; set; }

        public ProfilePageModel(AuthService authService)
        {
            _authService = authService;
            LogoutCommand = new AsyncCommand(Logout);
        }

        async Task Logout()
        {
            if (await _authService.SignOutAsync())
            {
                Application.Current.MainPage = FreshPageModelResolver.ResolvePageModel<IntroPageModel>();
            }
        }
    }
}
