using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using MVP.Helpers;
using MVP.Models;
using MVP.Services.Helpers;
using MVP.Services.Interfaces;
using Refit;
using Xamarin.Essentials;

namespace MVP.Services
{
    public class MvpApiService : IMvpApiService
    {
        readonly IMvpApi api;
        readonly IAnalyticsService analyticsService;

        public MvpApiService(IAnalyticsService analyticsService)
        {
            this.analyticsService = analyticsService;
            api = RestService.For<IMvpApi>(new HttpClient(new AuthenticatedHttpClientHandler(GetToken))
            {
                BaseAddress = new Uri("https://mvpapi.azure-api.net/mvp/api/")
            });
        }

        async Task<string> GetToken()
        {
            var token = await SecureStorage.GetAsync(Constants.AccessToken);
            return token;
        }

        /// <summary>
        /// Clears all locally stored data.
        /// </summary>
        /// <returns></returns>
        public async Task ClearAllLocalData()
        {
            await BlobCache.LocalMachine.Invalidate("profile");
            await BlobCache.LocalMachine.Invalidate("avatar");
        }

        /// <summary>
        /// Returns the profile data of the currently signed in MVP
        /// </summary>
        /// <returns>The MVP's profile information</returns>
        public Task<Profile> GetProfileAsync(bool forceRefresh = false)
        {
            try
            {
                return GetFromCacheAndGetLatest("profile", GetRemoteProfileAsync, new TimeSpan(14, 0, 0, 0), forceRefresh);
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Returns the latest profile data from the API.
        /// </summary>
        /// <returns></returns>
        async Task<Profile> GetRemoteProfileAsync()
        {
            try
            {
                return await api.GetProfile();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Get the profile picture of the currently signed in MVP
        /// </summary>
        /// <returns>JPG image byte array</returns>
        public async Task<string> GetProfileImageAsync(bool forceRefresh = false)
        {
            try
            {
                // Get rid of cached data.
                if (forceRefresh)
                    await BlobCache.LocalMachine.InvalidateObject<string>("avatar");

                // Grab cached data and return immediately + fetch new data in background if needed.
                var cachedProfileImage = BlobCache.LocalMachine.GetAndFetchLatest("avatar", GetRemoteProfileImageAsync,
                    offset =>
                    {
                        var elapsed = DateTimeOffset.Now - offset;
                        return elapsed > new TimeSpan(days: 14, hours: 0, minutes: 0, seconds: 0);
                    });

                var image = await cachedProfileImage.FirstOrDefaultAsync();
                return $"data:image/png;base64,{image}";
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Returns the latest profile image from the API.
        /// </summary>
        /// <returns></returns>
        async Task<string> GetRemoteProfileImageAsync()
        {
            try
            {
                var image = await api.GetProfileImage();
                image = image.TrimStart('"').TrimEnd('"');

                return image;
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Gets the MVPs activities, depending on the offset (page) and the limit (number of items per-page)
        /// </summary>
        /// <param name="offset">page to return</param>
        /// <param name="limit">number of items for the page</param>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>A list of the MVP's contributions</returns>
        public async Task<ContributionList> GetContributionsAsync(int offset = 0, int limit = 0, bool forceRefresh = false)
        {
            try
            {
                // Get rid of cached data.
                if (forceRefresh)
                    await BlobCache.LocalMachine.InvalidateObject<ContributionList>("contributions");

                // Grab cached data and return immediately + fetch new data in background if needed.
                var cachedContributions = BlobCache.LocalMachine.GetAndFetchLatest("contributions", () => GetRemoteContributionsAsync(offset, limit),
                    cacheOffset =>
                    {
                        var elapsed = DateTimeOffset.Now - cacheOffset;
                        return elapsed > new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0);
                    });

                var contributions = await cachedContributions.FirstOrDefaultAsync();

                return contributions;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Returns the latest contributions from the API.
        /// </summary>
        /// <returns></returns>
        async Task<ContributionList> GetRemoteContributionsAsync(int offset = 0, int limit = 0)
        {
            try
            {
                return await api.GetContributions(offset, limit);
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Submits a new contribution to the currently sign-in MVP profile
        /// </summary>
        /// <param name="contribution">The contribution to be submitted</param>
        /// <returns>Contribution submitted. This object should now have a valid ID and be added to the app's Contributions collection</returns>
        public async Task<Contribution> SubmitContributionAsync(Contribution contribution)
        {
            if (contribution == null)
                throw new NullReferenceException($"The {nameof(contribution)} parameter was null.");

            try
            {
                return await api.AddContribution(contribution);
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Updates an existing contribution, identified by the contribution ID
        /// </summary>
        /// <param name="contribution">Contribution to be updated</param>
        /// <returns>Bool to denote update success or failure</returns>
        public async Task<bool> UpdateContributionAsync(Contribution contribution)
        {
            if (contribution == null)
                throw new NullReferenceException($"The {nameof(contribution)} parameter was null.");

            try
            {
                await api.UpdateContribution(contribution);
                return true;
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return false;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return false;
            }
        }

        /// <summary>
        /// Delete contribution
        /// </summary>
        /// <param name="contribution">Item to delete</param>
        /// <returns>Success or failure</returns>
        public async Task<bool> DeleteContributionAsync(Contribution contribution)
        {
            if (contribution == null)
                throw new NullReferenceException($"The {nameof(contribution)} parameter was null.");

            try
            {
                await api.DeleteContribution(contribution.ContributionId ?? 0);

                return true;
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return false;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return false;
            }
        }

        /// <summary>
        /// This gets a list if the different contributions types
        /// </summary>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>List of contributions types</returns>
        public async Task<IReadOnlyList<ContributionType>> GetContributionTypesAsync(bool forceRefresh = false)
        {
            return await GetFromCacheOrRemote(
                "contributiontypes",
                GetRemoteContributionTypesAsync,
                DateTimeOffset.Now.AddYears(1)
            );
        }

        async Task<IReadOnlyList<ContributionType>> GetRemoteContributionTypesAsync()
        {
            try
            {
                return await api.GetContributionTypes();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Gets a list of the Contribution Technologies (aka Contribution Areas)
        /// </summary>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>A list of available contribution areas</returns>
        public async Task<IReadOnlyList<ContributionCategory>> GetContributionAreasAsync(bool forceRefresh = false)
        {
            return await GetFromCacheOrRemote(
                "contributionareas",
                GetRemoteContributionAreasAsync,
                DateTimeOffset.Now.AddYears(1)
            );
        }

        async Task<IReadOnlyList<ContributionCategory>> GetRemoteContributionAreasAsync()
        {
            try
            {
                return await api.GetContributionAreas();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Gets a list of contribution visibility options (aka Sharing Preferences). The traditional results are "Microsoft Only", "MVP Community" and "Everyone"
        /// </summary>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>A list of available visibilities</returns>
        public async Task<IReadOnlyList<Visibility>> GetVisibilitiesAsync(bool forceRefresh = false)
        {
            return await GetFromCacheOrRemote(
                "visibilities",
                GetRemoteVisibilitiesAsync,
                DateTimeOffset.Now.AddYears(1)
            );
        }

        async Task<IReadOnlyList<Visibility>> GetRemoteVisibilitiesAsync()
        {
            try
            {
                return await api.GetVisibilities();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return null;
            }
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

        void HandleApiException(ApiException e)
        {
            analyticsService.Report(e);

            if (e.StatusCode == HttpStatusCode.Unauthorized || e.StatusCode == HttpStatusCode.Forbidden)
            {
                HandleAccessTokenExpired();
            }
            else
            {
                HandleRequestErrorOccurred(e.Content, isBadRequest: true);
            }
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

        /// <summary>
        /// Grabs a piece of data from the cache or from remote depending on whether or not it exists.
        /// </summary>
        async Task<T> GetFromCacheOrRemote<T>(string key, Func<Task<T>> fetch, DateTimeOffset cacheDuration, bool forceRefresh = false)
        {
            try
            {
                // Get rid of cached data.
                if (forceRefresh)
                    await BlobCache.LocalMachine.InvalidateObject<T>(key);

                // Grab our data from cache.
                var cachedData = await BlobCache.LocalMachine
                    .GetOrCreateObject<T>(key, () => default(T), cacheDuration);

                // If the data was in cache, return it.
                if (cachedData != null)
                    return cachedData;

                // If not, grab it from remote, add to cache and return.
                var remoteData = await fetch();
                await BlobCache.LocalMachine.InsertObject(key, remoteData);

                return remoteData;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return default(T);
            }
        }

        /// <summary>
        /// Grabs a piece of data from the cache and subsequently starts a fetch from remote depending on whether or not it is still valid.
        /// </summary>
        async Task<T> GetFromCacheAndGetLatest<T>(string key, Func<Task<T>> fetch, TimeSpan cacheTimeSpan, bool forceRefresh)
        {
            try
            {
                // Get rid of cached data.
                if (forceRefresh)
                    await BlobCache.LocalMachine.InvalidateObject<T>(key);

                // Grab cached data and return immediately + fetch new data in background if needed.
                var cachedData = BlobCache.LocalMachine.GetAndFetchLatest(key, fetch,
                    offset =>
                    {
                        var elapsed = DateTimeOffset.Now - offset;
                        return elapsed > cacheTimeSpan;
                    });

                var data = await cachedData.FirstOrDefaultAsync();

                return data;
            }
            catch (Exception e)
            {
                analyticsService.Report(e);
                return default;
            }
        }
    }
}