using System;
using MVP.Models.Enums;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP
{
    public class Constants
    {
        // TODO: Re-enable when we move to MSAL ; public static readonly string[] AuthScopes = { "User.Read" };
        public static readonly string[] AuthScopes = { "wl.emails", "wl.basic", "wl.offline_access", "wl.signin" };
        public const string AuthType = "Bearer";
        public const string AppName = "MVPBuzz App";
        public const string AccessToken = "AccessToken";
        public const string RefreshToken = "RefreshToken";
        public const string SponsorUrl = "https://github.com/sponsors/sthewissen";
        public const string ApiUrl = "https://mvpapi.azure-api.net/mvp/api/";

        // This is needed for "old style" login through Live ID. Unfortunately.
        public const string RedirectUrl = "https://login.live.com/oauth20_desktop.srf";
        public const string AccessTokenUrl = "https://login.live.com/oauth20_token.srf";
        public static Uri SignInUrl = new Uri($"https://login.live.com/oauth20_authorize.srf?client_id={Helpers.Secrets.AuthClientId}&redirect_uri=https:%2F%2Flogin.live.com%2Foauth20_desktop.srf&response_type=code&scope={string.Join("%20", AuthScopes)}");
        public static Uri SignOutUri = new Uri($"https://login.live.com/oauth20_logout.srf?client_id={Helpers.Secrets.AuthClientId}&redirect_uri=https:%2F%2Flogin.live.com%2Foauth20_desktop.srf");
    }

    public static class StateKeys
    {
        public const string Offline = "Offline";
    }

    public class MessageKeys
    {
        public const string HardRefreshNeeded = "RefreshNeeded";
        public const string InMemoryUpdate = "InMemoryUpdate";
        public const string InMemoryAdd = "InMemoryAdd";
        public const string InMemoryDelete = "InMemoryDelete";
    }

    public class CacheKeys
    {
        public const string Contributions = "contributions";
        public const string ContributionTypes = "contributiontypes";
        public const string ContributionAreas = "contributionareas";
        public const string Visibilities = "visibilities";
        public const string Profile = "profile";
        public const string Avatar = "avatar";
        public const string FirstPageContributions = "firstPageContributions";
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
