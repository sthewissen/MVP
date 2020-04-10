using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Services.Helpers;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace MVP.Services
{
    public class MvpApiService : IDisposable, IMvpApiService
    {
        private readonly HttpClient _client;
        private ContributionList _contributionsCachedResult;
        private IReadOnlyList<ContributionType> _contributionTypesCachedResult;
        private IReadOnlyList<ContributionCategory> _contributionAreasCachedResult;
        private IReadOnlyList<Visibility> _visibilitiesCachedResult;
        private IReadOnlyList<OnlineIdentity> _onlineIdentitiesCachedResult;

        /// <summary>
        /// Service that interacts with the MVP API
        /// </summary>
        public MvpApiService()
        {
            var handler = new HttpClientHandler();

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri("https://mvpapi.azure-api.net/mvp/api/");
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Constants.OcpApimSubscriptionKey);

            // TODO: Improve, because I'm not too proud of this.
            var authorizationHeaderContent = SecureStorage.GetAsync("AccessToken").GetAwaiter().GetResult();

            if (authorizationHeaderContent.StartsWith(Constants.AuthType))
            {
                authorizationHeaderContent = authorizationHeaderContent.Replace($"{Constants.AuthType} ", string.Empty);
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AuthType, authorizationHeaderContent);
        }

        /// <summary>
        /// Returns the profile data of the currently signed in MVP
        /// </summary>
        /// <returns>The MVP's profile information</returns>
        public async Task<Profile> GetProfileAsync()
        {
            try
            {
                using (var response = await _client.GetAsync("profile"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<Profile>(json);
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        HandleRequestErrorOccurred(message, isBadRequest: true);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetProfileAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"GetProfileAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Get the profile picture of the currently signed in MVP
        /// </summary>
        /// <returns>JPG image byte array</returns>
        public async Task<string> GetProfileImageAsync()
        {
            try
            {
                // the result is Detected mime type: image/jpeg; charset=binary
                using (var response = await _client.GetAsync("profile/photo"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var base64String = await response.Content.ReadAsStringAsync();
                        base64String = base64String.TrimStart('"').TrimEnd('"');

                        return $"data:image/png;base64,{base64String}";
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            HandleAccessTokenExpired();
                        }
                        else if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            HandleRequestErrorOccurred(message, isBadRequest: true);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetProfileImageAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();
                Debug.WriteLine($"GetProfileImageAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Downloads and saves the image file to LocalFolder
        /// </summary>
        /// <returns>File path</returns>
        public async Task<string> DownloadAndSaveProfileImage()
        {
            try
            {
                // the result is Detected mime type: image/jpeg; charset=binary
                using (var response = await _client.GetAsync("profile/photo"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var base64String = await response.Content.ReadAsStringAsync();

                        try
                        {
                            if (string.IsNullOrEmpty(base64String))
                            {
                                return null;
                            }

                            base64String = base64String.TrimStart('"').TrimEnd('"');

                            // determine file type
                            var data = base64String.Substring(0, 5);

                            var fileExtension = string.Empty;

                            switch (data.ToUpper())
                            {
                                case "IVBOR":
                                    fileExtension = "png";
                                    break;
                                case "/9J/4":
                                    fileExtension = "jpg";
                                    break;
                            }

                            var imgBytes = Convert.FromBase64String(base64String);

                            // TODO: Fix storing the image.
                            // var filePath = StorageHelpers.Instance.SaveImage(imgBytes, $"ProfilePicture.{fileExtension}");
                            // return filePath;
                        }
                        catch (Exception e)
                        {
                            await e.LogExceptionAsync();
                            Debug.WriteLine($"DownloadAndSaveProfileImage Exception: {e}");
                        }
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            HandleAccessTokenExpired();
                        }
                        else if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            HandleRequestErrorOccurred(message, isBadRequest: true);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetProfileImageAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();
                Debug.WriteLine($"GetProfileImageAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Gets all the MVP's activities.
        /// </summary>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>A list of the MVP's contributions</returns>
        public async Task<ContributionList> GetAllContributionsAsync(bool forceRefresh = false)
        {
            if (_contributionsCachedResult != null && !forceRefresh)
            {
                // Return the cached result by default.
                return _contributionsCachedResult;
            }

            try
            {
                int totalCount = 0;

                // The first fetch gets the total count, which we need to do the full fetch
                using (var response = await _client.GetAsync($"contributions/0/0"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var deserializedResult = JsonConvert.DeserializeObject<ContributionList>(json);

                        // Read the total
                        totalCount = Convert.ToInt32(deserializedResult.TotalContributions);
                    }
                }

                // Using the total count, we can now fetch all the items and cache them
                return await GetContributionsAsync(0, totalCount, true);
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetContributionsAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"GetContributionsAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Gets the MVPs activities, depending on the offset (page) and the limit (number of items per-page)
        /// </summary>
        /// <param name="offset">page to return</param>
        /// <param name="limit">number of items for the page</param>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>A list of the MVP's contributions</returns>
        public async Task<ContributionList> GetContributionsAsync(int? offset, int limit, bool forceRefresh = false)
        {
            if (_contributionsCachedResult != null && !forceRefresh)
            {
                // Return the cached result by default.
                return _contributionsCachedResult;
            }

            if (offset == null)
            {
                offset = 0;
            }

            try
            {
                using (var response = await _client.GetAsync($"contributions/{offset}/{limit}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var deserializedResult = JsonConvert.DeserializeObject<ContributionList>(json);

                        // Update the cached result.
                        _contributionsCachedResult = deserializedResult;

                        return _contributionsCachedResult;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        HandleRequestErrorOccurred(message, isBadRequest: true);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetContributionsAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"GetContributionsAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Submits a new contribution to the currently sign-in MVP profile
        /// </summary>
        /// <param name="contribution">The contribution to be submitted</param>
        /// <returns>Contribution submitted. This object should now have a valid ID and be added to the app's Contributions collection</returns>
        public async Task<Contribution> SubmitContributionAsync(Contribution contribution)
        {
            if (contribution == null)
                throw new NullReferenceException("The contribution parameter was null.");

            try
            {
                var serializedContribution = JsonConvert.SerializeObject(contribution);
                byte[] byteData = Encoding.UTF8.GetBytes(serializedContribution);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    using (var response = await _client.PostAsync("contributions?", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<Contribution>(json);
                        }

                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            HandleAccessTokenExpired();
                        }
                        else if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            HandleRequestErrorOccurred(message, isBadRequest: true);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"SubmitContributionAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();
                Debug.WriteLine($"SubmitContributionAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Updates an existing contribution, identified by the contribution ID
        /// </summary>
        /// <param name="contribution">Contribution to be updated</param>
        /// <returns>Bool to denote update success or failure</returns>
        public async Task<bool?> UpdateContributionAsync(Contribution contribution)
        {
            if (contribution == null)
                throw new NullReferenceException("The contribution parameter was null.");

            try
            {
                // Request body
                var serializedContribution = JsonConvert.SerializeObject(contribution);
                byte[] byteData = Encoding.UTF8.GetBytes(serializedContribution);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    using (var response = await _client.PutAsync("contributions?", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return true;
                        }

                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            HandleAccessTokenExpired();
                        }
                        else if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            HandleRequestErrorOccurred(message, isBadRequest: true);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"UpdateContributionAsync HttpRequestException: {e}");
                return null;
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"GetProfileAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Delete contribution
        /// </summary>
        /// <param name="contribution">Item to delete</param>
        /// <returns>Success or failure</returns>
        public async Task<bool?> DeleteContributionAsync(Contribution contribution)
        {
            if (contribution == null)
                throw new NullReferenceException("The contribution parameter was null.");

            try
            {
                using (var response = await _client.DeleteAsync($"contributions?id={contribution.ContributionId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        HandleRequestErrorOccurred(message, isBadRequest: true);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"UpdateContributionAsync HttpRequestException: {e}");
                return null;
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"GetProfileAsync Exception: {e}");
                return null;
            }

            return null;
        }

        /// <summary>
        /// This gets a list if the different contributions types
        /// </summary>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>List of contributions types</returns>
        public async Task<IReadOnlyList<ContributionType>> GetContributionTypesAsync(bool forceRefresh = false)
        {
            if (_contributionTypesCachedResult?.Count == 0 && !forceRefresh)
            {
                // Return the cached result by default.
                return _contributionTypesCachedResult;
            }

            try
            {
                using (var response = await _client.GetAsync("contributions/contributiontypes"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var deserializedResult = JsonConvert.DeserializeObject<IReadOnlyList<ContributionType>>(json);

                        // Update the cached result.
                        _contributionTypesCachedResult = new List<ContributionType>(deserializedResult);

                        return _contributionTypesCachedResult;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        HandleRequestErrorOccurred(message, isBadRequest: true);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetContributionTypesAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"GetContributionTypesAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Gets a list of the Contribution Technologies (aka Contribution Areas)
        /// </summary>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>A list of available contribution areas</returns>
        public async Task<IReadOnlyList<ContributionCategory>> GetContributionAreasAsync(bool forceRefresh = false)
        {
            if (_contributionAreasCachedResult?.Count == 0 && !forceRefresh)
            {
                // Return the cached result by default.
                return _contributionAreasCachedResult;
            }

            try
            {
                using (var response = await _client.GetAsync("contributions/contributionareas"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var deserializedResult = JsonConvert.DeserializeObject<IReadOnlyList<ContributionCategory>>(json);

                        // Update the cached result.
                        _contributionAreasCachedResult = new List<ContributionCategory>(deserializedResult);

                        return _contributionAreasCachedResult;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        HandleRequestErrorOccurred(message, isBadRequest: true);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetContributionTechnologiesAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"GetContributionTechnologiesAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Gets a list of contribution visibility options (aka Sharing Preferences). The traditional results are "Microsoft Only", "MVP Community" and "Everyone"
        /// </summary>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>A list of available visibilities</returns>
        public async Task<IReadOnlyList<Visibility>> GetVisibilitiesAsync(bool forceRefresh = false)
        {
            if (_visibilitiesCachedResult?.Count == 0 && !forceRefresh)
            {
                // Return the cached result by default.
                return _visibilitiesCachedResult;
            }

            try
            {
                using (var response = await _client.GetAsync("contributions/sharingpreferences"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        var deserializedResult = JsonConvert.DeserializeObject<IReadOnlyList<Visibility>>(json);

                        // Update the cached result.
                        _visibilitiesCachedResult = new List<Visibility>(deserializedResult);

                        return _visibilitiesCachedResult;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        RequestErrorOccurred?.Invoke(this, new ApiServiceEventArgs { IsBadRequest = true, Message = "Bad Request Error - If this continues to happen, please open a GitHub issue so we can fix this immediately (go to the About page for a direct link)." });
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetVisibilitiesAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"GetVisibilitiesAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Returns a list of the MVP's OnlineIdentities (social media accounts and other identities)
        /// </summary>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns></returns>
        public async Task<IReadOnlyList<OnlineIdentity>> GetOnlineIdentitiesAsync(bool forceRefresh = false)
        {
            if (_contributionTypesCachedResult?.Count == 0 && !forceRefresh)
            {
                // Return the cached result by default.
                return _onlineIdentitiesCachedResult;
            }

            try
            {
                using (var response = await _client.GetAsync("onlineidentities"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var deserializedResult = JsonConvert.DeserializeObject<IReadOnlyList<OnlineIdentity>>(json);

                        // Update the cached result.
                        _onlineIdentitiesCachedResult = new List<OnlineIdentity>(deserializedResult);

                        return _onlineIdentitiesCachedResult;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        HandleRequestErrorOccurred(message, isBadRequest: true);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetOnlineIdentitiesAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();
                Debug.WriteLine($"GetOnlineIdentitiesAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Saves an OnlineIdentity
        /// </summary>
        /// <param name="onlineIdentity"></param>
        /// <returns></returns>
        public async Task<OnlineIdentity> SubmitOnlineIdentityAsync(OnlineIdentity onlineIdentity)
        {
            if (onlineIdentity == null)
                throw new NullReferenceException("The OnlineIdentity parameter was null.");

            try
            {
                var serializedOnlineIdentity = JsonConvert.SerializeObject(onlineIdentity);
                byte[] byteData = Encoding.UTF8.GetBytes(serializedOnlineIdentity);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    using (var response = await _client.PostAsync("onlineidentities?", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            Debug.WriteLine($"OnlineIdentity Save JSON: {json}");

                            var result = JsonConvert.DeserializeObject<OnlineIdentity>(json);
                            Debug.WriteLine($"OnlineIdentity Save Result: ID {result.PrivateSiteId}");

                            return result;
                        }

                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            HandleAccessTokenExpired();
                        }
                        else if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            HandleRequestErrorOccurred(message, isBadRequest: true);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"SubmitOnlineIdentitiesAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"SubmitOnlineIdentitiesAsync Exception: {e}");
            }

            return null;
        }

        public async Task<bool> DeleteOnlineIdentityAsync(OnlineIdentity onlineIdentity)
        {
            if (onlineIdentity == null)
                throw new NullReferenceException("The OnlineIdentity parameter was null.");

            try
            {
                using (var response = await _client.DeleteAsync($"onlineidentities?id={onlineIdentity.PrivateSiteId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        HandleRequestErrorOccurred(message, isBadRequest: true);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"SubmitOnlineIdentitiesAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();

                Debug.WriteLine($"SubmitOnlineIdentitiesAsync Exception: {e}");
            }

            return false;
        }

        /// <summary>
        /// Gets the current Award Consideration Questions list.
        /// </summary>
        /// <returns>The list of questions to be answered for consideration in the next award period.</returns>
        public async Task<IReadOnlyList<AwardConsiderationQuestion>> GetAwardConsiderationQuestionsAsync()
        {
            try
            {
                using (var response = await _client.GetAsync("awardconsideration/getcurrentquestions"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<IReadOnlyList<AwardConsiderationQuestion>>(json);
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        HandleRequestErrorOccurred(message, isBadRequest: true);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetOnlineIdentitiesAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();
                Debug.WriteLine($"GetOnlineIdentitiesAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Gets the MVP's currently saved answers for the Award consideration questions
        /// </summary>
        /// <returns>The list of questions to be answered for consideration in the next award period.</returns>
        public async Task<IReadOnlyList<AwardConsiderationAnswer>> GetAwardConsiderationAnswersAsync()
        {
            try
            {
                using (var response = await _client.GetAsync("awardconsideration/GetAnswers"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<IReadOnlyList<AwardConsiderationAnswer>>(json);
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleAccessTokenExpired();
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        HandleRequestErrorOccurred(message, isBadRequest: true);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"GetOnlineIdentitiesAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();
                Debug.WriteLine($"GetOnlineIdentitiesAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Saves the MVP's answers to the Aware Consideration questions.
        /// IMPORTANT NOTE:
        /// This does NOT submit them for review by the MVP award team, it is intended to be used to save the answers.
        /// To submit the questions, call SubmitAwardConsiderationAnswerAsync after saving the answers.
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public async Task<List<AwardConsiderationAnswer>> SaveAwardConsiderationAnswerAsync(IEnumerable<AwardConsiderationAnswer> answers)
        {
            if (answers == null)
                throw new NullReferenceException("The contribution parameter was null.");

            try
            {
                var serializedContribution = JsonConvert.SerializeObject(answers);
                byte[] byteData = Encoding.UTF8.GetBytes(serializedContribution);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    using (var response = await _client.PostAsync("awardconsideration/saveanswers?", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<List<AwardConsiderationAnswer>>(json);
                        }

                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            HandleAccessTokenExpired();
                        }
                        else if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            HandleRequestErrorOccurred(message, isBadRequest: true);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(string.Empty, isServerError: true);
                }

                Debug.WriteLine($"SubmitContributionAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();
                Debug.WriteLine($"SubmitContributionAsync Exception: {e}");
            }

            return null;
        }

        /// <summary>
        /// Submits the MVP's answers for award consideration questions.
        /// WARNING - THIS CAN ONLY BE DONE ONCE PER AWARD PERIOD, THE ANSWERS CANNOT BE CHANGED AFTER SUBMISSION.
        /// </summary>
        /// <returns>If the submission was successful</returns>
        public async Task<bool> SubmitAwardConsiderationAnswerAsync()
        {
            try
            {
                var serializedContribution = JsonConvert.SerializeObject(string.Empty);
                byte[] byteData = Encoding.UTF8.GetBytes(serializedContribution);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    using (var response = await _client.PostAsync("awardconsideration/SubmitAnswers?", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return true;
                        }

                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            HandleAccessTokenExpired();
                        }
                        else if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            HandleRequestErrorOccurred(message, isBadRequest: true);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await e.LogExceptionAsync();

                if (e.Message.Contains("500"))
                {
                    HandleRequestErrorOccurred(e.Message, isServerError: true);
                }

                Debug.WriteLine($"SubmitContributionAsync HttpRequestException: {e}");
            }
            catch (Exception e)
            {
                await e.LogExceptionAsync();
                Debug.WriteLine($"SubmitContributionAsync Exception: {e}");
            }

            return false;
        }

        void HandleRequestErrorOccurred(string message, bool isServerError = false, bool isBadRequest = false, bool isTokenRefreshNeeded = false)
        {
            RequestErrorOccurred?.Invoke(this, new ApiServiceEventArgs
            {
                IsServerError = isServerError,
                IsBadRequest = isBadRequest,
                IsTokenRefreshNeeded = isTokenRefreshNeeded,
                Message = message
            });
        }

        void HandleAccessTokenExpired()
        {
            AccessTokenExpired?.Invoke(this, new ApiServiceEventArgs { IsTokenRefreshNeeded = true });
        }

        /// <summary>
        /// This event will fire when there is a 401 or 403 returned from an API call. This indicates that a new Access Token is needed.
        /// Use this event to use the refresh token to get a new access token automatically.
        /// </summary>
        public event EventHandler<ApiServiceEventArgs> AccessTokenExpired;

        /// <summary>
        /// This event fires when the API call results in a HttpStatusCode 500 result is obtained.
        /// </summary>
        public event EventHandler<ApiServiceEventArgs> RequestErrorOccurred;

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}