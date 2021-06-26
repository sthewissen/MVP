using MVP.Models.Enums;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP
{
    public class Constants
    {
        public static readonly string[] AuthScopes = { "wl.signin", "wl.emails" };
        public const string AuthType = "Bearer";
        public const string AppName = "MVPBuzz";
        public const string AccessToken = "AccessToken";
        public const string SponsorUrl = "https://github.com/sponsors/sthewissen";
        public const string ApiUrl = "https://mvpapi.azure-api.net/mvp/api/";
    }

    public static class StateKeys
    {
        public const string Offline = "Offline";
    }

    public class MessageKeys
    {
        public const string RefreshNeeded = "RefreshNeeded";
        public const string InMemoryUpdate = "InMemoryUpdate";
    }

    public class CacheKeys
    {
        public const string Contributions = "contributions";
        public const string ContributionTypes = "contributiontypes";
        public const string ContributionAreas = "contributionareas";
        public const string Visibilities = "visibilities";
        public const string Profile = "profile";
        public const string Avatar = "avatar";
    }

    public class Settings
    {
        public static bool UseClipboardUrls
        {
            get => Preferences.Get(nameof(UseClipboardUrls), true);
            set => Preferences.Set(nameof(UseClipboardUrls), value);
        }

        public static OSAppTheme AppTheme
        {
            get => (OSAppTheme)Preferences.Get(nameof(AppTheme), 0);
            set => Preferences.Set(nameof(AppTheme), (int)value);
        }

        public static string AppLanguage
        {
            get => Preferences.Get(nameof(AppLanguage), "en");
            set => Preferences.Set(nameof(AppLanguage), value);
        }

        public static AppIcon AppIcon
        {
            get => (AppIcon)Preferences.Get(nameof(AppIcon), 0);
            set => Preferences.Set(nameof(AppIcon), (int)value);
        }

        public static int StartupCount
        {
            get => Preferences.Get(nameof(StartupCount), 0);
            set => Preferences.Set(nameof(StartupCount), value);
        }

        public static bool IsUsingDemoAccount
        {
            get => Preferences.Get(nameof(IsUsingDemoAccount), false);
            set => Preferences.Set(nameof(IsUsingDemoAccount), value);
        }
    }
}
