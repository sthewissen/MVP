using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using MVP.Services.Helpers;
using Xamarin.Essentials;

namespace MVP.Services
{
    public class AuthService
    {
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

        readonly string[] Scopes = { "wl.signin", "wl.emails" };
        readonly IPublicClientApplication _pca;

        // Android uses this to determine which activity to use to show
        // the login screen dialog from.
        public static object ParentWindow { get; set; }

        public AuthService()
        {
            _pca = PublicClientApplicationBuilder.Create(Constants.AuthClientId)
                .WithIosKeychainSecurityGroup(AppInfo.PackageName)
                .WithRedirectUri(RedirectUri)
                .WithAuthority("https://login.microsoftonline.com/common")
                .Build();
        }

        public async Task<bool> SignInSilentAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await _pca.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();

                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

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
                var accounts = await _pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await _pca.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();

                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                return true;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult = await _pca.AcquireTokenInteractive(Scopes)
                                                .WithParentActivityOrWindow(ParentWindow)
                                                .WithUseEmbeddedWebView(true)
                                                .ExecuteAsync();

                    // Store the access token securely for later use.
                    await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                    return true;
                }
                catch (Exception ex)
                {
                    ex.LogException();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
                return false;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await _pca.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await _pca.GetAccountsAsync();
                }

                // Clear our access token from secure storage.
                SecureStorage.Remove("AccessToken");

                return true;
            }
            catch (Exception ex)
            {
                ex.LogException();
                return false;
            }
        }
    }
}
