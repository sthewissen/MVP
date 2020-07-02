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
        readonly IAuthService _authService;

        public IAsyncCommand LogoutCommand { get; set; }

        public ProfilePageModel(IAuthService authService)
        {
            _authService = authService;
            LogoutCommand = new AsyncCommand(() => Logout());
        }

        async Task Logout()
        {
            if (await _authService.SignOutAsync())
            {
                await MvpApiService.ClearAllLocalData();
                Application.Current.MainPage = FreshPageModelResolver.ResolvePageModel<IntroPageModel>();
            }
        }
    }
}
