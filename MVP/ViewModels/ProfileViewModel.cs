using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Extensions;
using MVP.Models;
using MVP.Pages;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
using TinyMvvm;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public IAsyncCommand LogoutCommand { get; set; }
        public IAsyncCommand LoadProfileCommand { get; set; }
        public IAsyncCommand OpenThemePickerCommand { get; set; }
        public IAsyncCommand OpenIconPickerCommand { get; set; }
        public IAsyncCommand OpenLanguagePickerCommand { get; set; }
        public IAsyncCommand OpenAboutCommand { get; set; }
        public ICommand ToggleUseClipboardUrlsCommand { get; set; }

        public Profile Profile { get; set; }
        public string ProfileImage { get; set; }
        public string AppVersion => $"v{AppInfo.VersionString}";

        public bool UseClipboardUrls
        {
            get => Preferences.Get(Settings.UseClipboardUrls, true);
            set
            {
                Preferences.Set(Settings.UseClipboardUrls, value);
                HapticFeedback.Perform(HapticFeedbackType.Click);
            }
        }

        public ProfileViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
            LogoutCommand = new AsyncCommand(() => Logout());
            LoadProfileCommand = new AsyncCommand(() => LoadProfile(true));
            ToggleUseClipboardUrlsCommand = new Command(() => UseClipboardUrls = !UseClipboardUrls);
            OpenThemePickerCommand = new AsyncCommand(() => OpenThemePicker());
            OpenLanguagePickerCommand = new AsyncCommand(() => OpenLanguagePicker());
            OpenAboutCommand = new AsyncCommand(() => OpenAbout());
            OpenIconPickerCommand = new AsyncCommand(() => OpenIconPicker(), (o) => Device.RuntimePlatform == Device.iOS);
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            LoadProfile(false).SafeFireAndForget();
        }

        async Task LoadProfile(bool force)
        {
            if (State != LayoutState.None)
                return;

            if (force && Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Connection to internet is not available
                await DialogService.AlertAsync(
                    Translations.error_offline,
                    Translations.error_offline_title,
                    Translations.alert_ok).ConfigureAwait(false);
                return;
            }

            State = LayoutState.Loading;
            await Task.WhenAll(RefreshProfileData(force), RefreshProfileImage(force));
            State = LayoutState.None;
        }

        async Task RefreshProfileImage(bool force)
        {
            var image = await MvpApiService.GetProfileImageAsync(force).ConfigureAwait(false);

            if (image == null && force)
                return;

            ProfileImage = image;
        }

        async Task RefreshProfileData(bool force)
        {
            var profile = await MvpApiService.GetProfileAsync(force).ConfigureAwait(false);

            if (profile == null && force)
                return;

            Profile = profile;
        }

        async Task Logout()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Connection to internet is not available
                await DialogService.ConfirmAsync(
                    Translations.error_logoutwhileoffline,
                    Translations.error_offline_title,
                    Translations.alert_yes,
                    Translations.alert_no).ConfigureAwait(false);
                return;
            }

            if (!await AuthService.SignOutAsync())
                return;

            await MvpApiService.ClearAllLocalData();
            NavigationHelper.SetRootView(nameof(IntroPage));
        }

        async Task OpenThemePicker()
            => await NavigationHelper.NavigateToAsync(nameof(ThemePickerPage)).ConfigureAwait(false);

        async Task OpenLanguagePicker()
            => await NavigationHelper.NavigateToAsync(nameof(LanguagePickerPage)).ConfigureAwait(false);

        async Task OpenIconPicker()
            => await NavigationHelper.NavigateToAsync(nameof(AppIconPickerPage)).ConfigureAwait(false);

        async Task OpenAbout()
            => await NavigationHelper.NavigateToAsync(nameof(AboutPage)).ConfigureAwait(false);
    }
}
