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

        public const string AppCenterDeviOSKey = "71df115f-45d9-49da-bf13-de09ff4a3aff";
        public const string AppCenterDevAndroidKey = "36d4c425-d4b8-4712-baa3-681cc6586c14";
        public const string AppCenterProdiOSKey = "d93d1cb9-8a66-494a-8478-b73ac5f0516d";
        public const string AppCenterProdAndroidKey = "3aa628e6-2d4e-428a-b064-88a2d358763e";
    }
    }

    public class Settings
    {
        public const string UseClipboardUrls = "use_clipboard_urls";
        public const bool UseClipboardUrlsDefault = true;

        public const string AppTheme = "app_theme";
        public const int AppThemeDefault = 0;

        public const string AppLanguage = "app_language";
        public const string AppLanguageDefault = "en";

        public const string AppIcon = "app_icon";
        public const int AppIconDefault = 0;
    }

    public class Alerts
    {
        public const string OK = "OK";
        public const string Cancel = "Cancel";
        public const string HoldOn = "Hold on...";
        public const string Error = "That's not good...";
        public const string UnexpectedError = "Something went wrong that we didn't expect could happen. An error has been logged and we will look into it as soon as possible.";
    }
}
