using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using MVP.Services.Interfaces;
using Xamarin.Essentials;

namespace MVP.Services
{
    /// <summary>
    /// Authenticates the user using their Live ID.
    /// </summary>
    public class AuthService : IAuthService
    {
        readonly IAnalyticsService analyticsService;
        readonly IPublicClientApplication pca;

        // The redirect URI defines how the external browser window can
        // callback into our app after authentication has happened.
        string RedirectUri
        {
            get
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                    return $"msauth://{AppInfo.PackageName}/{Constants.AuthSignatureHash}";
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                    return $"msauth.{AppInfo.PackageName}://auth";

                return string.Empty;
            }
        }

        // Android uses this to determine which activity to use to show
        // the login screen dialog from.
        public static object ParentWindow { get; set; }

        public AuthService(IAnalyticsService analyticsService)
        {
            this.analyticsService = analyticsService;
            pca = PublicClientApplicationBuilder.Create(Constants.AuthClientId)
                .WithIosKeychainSecurityGroup(AppInfo.PackageName)
                .WithRedirectUri(RedirectUri)
                .WithAuthority("https://login.microsoftonline.com/common")
                .Build();
        }

        public async Task<bool> SignInSilentAsync()
        {
            try
            {
                var accounts = await pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await pca.AcquireTokenSilent(Constants.AuthScopes, firstAccount).ExecuteAsync();

                // Store the access token securely for later use.
                await SecureStorage.SetAsync(Constants.AccessToken, $"{Constants.AuthType} {authResult?.AccessToken}");

                return true;
            }
            catch (MsalUiRequiredException)
            {
                return false;
            }
        }

        public async Task<bool> SignInAsync()
        {
            try
            {
                var accounts = await pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await pca.AcquireTokenSilent(Constants.AuthScopes, firstAccount).ExecuteAsync();

                // Store the access token securely for later use.
                await SecureStorage.SetAsync(Constants.AccessToken, $"{Constants.AuthType} {authResult?.AccessToken}");

                return true;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult = await pca.AcquireTokenInteractive(Constants.AuthScopes)
                                                .WithParentActivityOrWindow(ParentWindow)
                                                .WithUseEmbeddedWebView(true)
                                                .ExecuteAsync();

                    // Store the access token securely for later use.
                    await SecureStorage.SetAsync(Constants.AccessToken, $"{Constants.AuthType} {authResult?.AccessToken}");

                    return true;
                }
                catch (Exception ex)
                {
                    analyticsService.Report(ex);
                    return false;
                }
            }
            catch (Exception ex)
            {
                analyticsService.Report(ex);
                return false;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await pca.GetAccountsAsync();

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await pca.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await pca.GetAccountsAsync();
                }

                // Clear our access token from secure storage.
                SecureStorage.Remove(Constants.AccessToken);

                return true;
            }
            catch (Exception ex)
            {
                analyticsService.Report(ex);
                return false;
            }
        }
    }
}
