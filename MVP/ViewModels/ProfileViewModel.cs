using System.Threading.Tasks;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public IAsyncCommand LogoutCommand { get; set; }
        public IAsyncCommand LoadProfileCommand { get; set; }
        public IAsyncCommand OpenThemePickerCommand { get; set; }

        public Profile Profile { get; set; }
        public string ProfileImage { get; set; }

        public bool UseClipboardUrls
        {
            get => Preferences.Get(Settings.UseClipboardUrls, true);
            set
            {
                Preferences.Set(Settings.UseClipboardUrls, value);
                Vibration.Vibrate(100);
            }
        }

        public ProfileViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            LogoutCommand = new AsyncCommand(() => Logout());
            LoadProfileCommand = new AsyncCommand(() => LoadProfile(true));
            OpenThemePickerCommand = new AsyncCommand(() => OpenThemePicker());
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            await LoadProfile(false);
        }

        async Task LoadProfile(bool force)
        {
            State = LayoutState.Loading;
            await Task.WhenAll(RefreshProfileData(force), RefreshProfileImage(force));
            State = LayoutState.None;
        }

        async Task RefreshProfileImage(bool force)
        {
            var image = await MvpApiService.GetProfileImageAsync(force).ConfigureAwait(false);

            if (image == null && force)
                return;

            if (image == null)
            {
                image = await MvpApiService.GetProfileImageAsync(true).ConfigureAwait(false);

                if (image == null)
                    return;
            }

            ProfileImage = image;
        }

        async Task RefreshProfileData(bool force)
        {
            var profile = await MvpApiService.GetProfileAsync(force).ConfigureAwait(false);

            if (profile == null && force)
                return;

            if (profile == null)
            {
                profile = await MvpApiService.GetProfileAsync(true).ConfigureAwait(false);

                if (profile == null)
                    return;
            }

            Profile = profile;
        }

        async Task Logout()
        {
            if (!await AuthService.SignOutAsync())
                return;

            await MvpApiService.ClearAllLocalData();
            NavigationHelper.SetRootView(nameof(IntroPage));
        }

        async Task OpenThemePicker()
            => await NavigationHelper.NavigateToAsync(nameof(ThemePickerPage)).ConfigureAwait(false);
    }
}
