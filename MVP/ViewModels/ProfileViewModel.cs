using System.Threading.Tasks;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public IAsyncCommand LogoutCommand { get; set; }

        public Profile Profile { get; set; }
        public string ProfileImage { get; set; }
        public string Name { get; set; }

        public ProfileViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            LogoutCommand = new AsyncCommand(() => Logout());
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await Task.WhenAll(RefreshProfileData(), RefreshProfileImage());
        }

        async Task RefreshProfileImage()
        {
            var image = await MvpApiService.GetProfileImageAsync().ConfigureAwait(false);

            if (image == null)
                return;

            ProfileImage = image;
        }

        async Task RefreshProfileData()
        {
            var profile = await MvpApiService.GetProfileAsync().ConfigureAwait(false);

            if (profile == null)
                return;

            Profile = profile;
        }

        async Task Logout()
        {
            if (await AuthService.SignOutAsync())
            {
                await MvpApiService.ClearAllLocalData();
                NavigationHelper.SetRootView(nameof(IntroPage));
            }
        }
    }
}
