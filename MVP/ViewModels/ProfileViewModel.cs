using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Extensions;
using MVP.Models;
using MVP.Pages;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
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

        public ProfileViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
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

        /// <summary>
        /// Loads the profile information.
        /// </summary>
        async Task LoadProfile(bool force)
        {
            try
            {
                if (State != LayoutState.None)
                    return;

                if (!await VerifyInternetConnection())
                    return;

                State = LayoutState.Loading;
                await Task.WhenAll(RefreshProfileData(force), RefreshProfileImage(force));
                State = LayoutState.None;

                if (force)
                    AnalyticsService.Track("Profile Refreshed");
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex);
                await DialogService.AlertAsync(Translations.error_couldntrefreshprofile, Translations.error_title, Translations.ok).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the profile image data.
        /// </summary>
        async Task RefreshProfileImage(bool force)
        {
            var image = await MvpApiService.GetProfileImageAsync(force).ConfigureAwait(false);

            if (image == null && force)
                return;

            ProfileImage = image;
        }

        /// <summary>
        /// Gets profile data.
        /// </summary>
        async Task RefreshProfileData(bool force)
        {
            var profile = await MvpApiService.GetProfileAsync(force).ConfigureAwait(false);

            if (profile == null && force)
                return;

            Profile = profile;
        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        /// <returns></returns>
        async Task Logout()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Connection to internet is not available
                await DialogService.ConfirmAsync(Translations.error_logoutwhileoffline, Translations.error_offline_title, Translations.yes, Translations.no).ConfigureAwait(false);
                return;
            }

            if (!await AuthService.SignOutAsync())
                return;

            await MvpApiService.ClearAllLocalData();
            NavigationHelper.SetRootView(nameof(IntroPage));
            AnalyticsService.Track("User Logged Out");
        }

        async Task OpenThemePicker()
            => await NavigateAsync(nameof(ThemePickerPage)).ConfigureAwait(false);

        async Task OpenLanguagePicker()
        {
            if (Device.RuntimePlatform == Device.iOS)
                AppInfo.ShowSettingsUI();
            else
                await NavigateAsync(nameof(LanguagePickerPage)).ConfigureAwait(false);
        }

        async Task OpenIconPicker()
            => await NavigateAsync(nameof(AppIconPickerPage)).ConfigureAwait(false);

        async Task OpenAbout()
            => await NavigateAsync(nameof(AboutPage)).ConfigureAwait(false);
    }
}
