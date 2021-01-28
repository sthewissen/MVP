using MVP.Models.Enums;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP
{
    public class Constants
    {
        public static readonly string[] AuthScopes = { "wl.signin", "wl.emails" };
        public const string AuthClientId = "c18f496a-94e7-4307-87f4-2a255314bb4c";
        public const string AuthSignatureHash = "AvlqWgVzwpWojk8fpGXIZrw3oIQ%3D";
        public const string AuthType = "Bearer";
        public const string AppName = "MVPBuzz";
        public const string AccessToken = "AccessToken";
        public const string SponsorUrl = "https://github.com/sponsors/sthewissen";
        public const string ApiUrl = "https://mvpapi.azure-api.net/mvp/api/";

        public const string AppCenterDeviOSKey = "71df115f-45d9-49da-bf13-de09ff4a3aff";
        public const string AppCenterDevAndroidKey = "36d4c425-d4b8-4712-baa3-681cc6586c14";
        public const string AppCenterProdiOSKey = "d93d1cb9-8a66-494a-8478-b73ac5f0516d";
        public const string AppCenterProdAndroidKey = "3aa628e6-2d4e-428a-b064-88a2d358763e";
    }

    public static class StateKeys
    {
        public const string Offline = "Offline";
    }

    public class MessageKeys
    {
        public const string RefreshNeeded = "RefreshNeeded";
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
