using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MVP.Services.Interfaces;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace MVP.Services
{
    public class LiveAuthService : IAuthService
    {
        readonly IAnalyticsService analyticsService;

        public LiveAuthService(IAnalyticsService analyticsService)
        {
            this.analyticsService = analyticsService;
        }

        public async Task<bool> SignInAsync()
        {
            var refreshToken = await SecureStorage.GetAsync(Constants.RefreshToken);

            // If refresh token is available, the user has previously been logged in and we can get a refreshed access token immediately
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var authorizationHeader = await RequestAuthorizationAsync(refreshToken, true);

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    // something went wrong using refresh token, use WebView to login
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                // No stored credentials, use WebView workflow
                return false;
            }
        }

        public async Task<bool> SignInSilentAsync()
        {
            var refreshToken = await SecureStorage.GetAsync(Constants.RefreshToken);

            // If refresh token is available, the user has previously been logged in and we can get a refreshed access token immediately
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var authorizationHeader = await RequestAuthorizationAsync(refreshToken, true);

                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    return true;
                }
            }

            return false;
        }

        public Task<bool> SignOutAsync()
        {
            try
            {
                // Erase cached tokens
                SecureStorage.Remove(Constants.AccessToken);
                SecureStorage.Remove(Constants.RefreshToken);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                analyticsService.Report(ex);
                return Task.FromResult(false);
            }
        }

        public async Task<string> RequestAuthorizationAsync(string authCode, bool isRefresh = false)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Construct the Form content, this is where I add the OAuth token (could be access token or refresh token)
                    var postContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("client_id", MVP.Helpers.Secrets.AuthClientId),
                        new KeyValuePair<string, string>("grant_type", isRefresh ? "refresh_token" : "authorization_code"),
                        new KeyValuePair<string, string>(isRefresh ? "refresh_token" : "code", authCode.Split('&')[0]),
                        new KeyValuePair<string, string>("redirect_uri", Constants.RedirectUrl)
                    });

                    // Variable to hold the response data
                    var responseTxt = "";

                    // post the Form data
                    using (var response = await client.PostAsync(new Uri(Constants.AccessTokenUrl), postContent))
                    {
                        // Read the response
                        responseTxt = await response.Content.ReadAsStringAsync();
                    }

                    // Deserialize the parameters from the response
                    var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseTxt);

                    if (tokenData.ContainsKey("access_token"))
                    {
                        await SecureStorage.SetAsync(Constants.RefreshToken, tokenData["refresh_token"]);

                        // We need to prefix the access token with the token type for the auth header. 
                        // Currently this is always "bearer", doing this to be more future proof
                        var tokenType = tokenData["token_type"];
                        var cleanedAccessToken = tokenData["access_token"].Split('&')[0];

                        // set public property that is "returned"
                        await SecureStorage.SetAsync(Constants.AccessToken, $"{tokenType} {cleanedAccessToken}");
                        return cleanedAccessToken;
                    }
                }
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }

            return null;
        }
    }
}
