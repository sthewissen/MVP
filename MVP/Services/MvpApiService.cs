using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        readonly IMvpApi _api;
        readonly IAnalyticsService _analyticsService;

        public MvpApiService(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            _api = RestService.For<IMvpApi>(new HttpClient(new AuthenticatedHttpClientHandler(GetToken))
            {
                BaseAddress = new Uri("https://mvpapi.azure-api.net/mvp/api/")
            });
        }

        async Task<string> GetToken()
        {
            var token = await SecureStorage.GetAsync("AccessToken");
            return token;
        }

        /// <summary>
        /// Returns the profile data of the currently signed in MVP
        /// </summary>
        /// <returns>The MVP's profile information</returns>
        public async Task<Profile> GetProfileAsync(bool forceRefresh = false)
        {
            try
            {
                // TODO: Caching

                return await _api.GetProfile();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
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
                var image = await _api.GetProfileImage();
                image = image.TrimStart('"').TrimEnd('"');

                return $"data:image/png;base64,{image}";
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="offset">page to return</param>
        /// <param name="limit">number of items for the page</param>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns>A list of the MVP's contributions</returns>
        public async Task<ContributionList> GetContributionsAsync(int offset = 0, int limit = 0, bool forceRefresh = false)
        {
            try
            {
                var image = await _api.GetProfileImage();
                image = image.TrimStart('"').TrimEnd('"');

                // TODO: Cache image locally

                return $"data:image/png;base64,{image}";
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
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
                // TODO: Do something with caching.

                return await _api.GetContributions(offset, limit);
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
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
                return await _api.AddContribution(contribution);
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
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
                await _api.UpdateContribution(contribution);
                return true;
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return false;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
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
                await _api.DeleteContribution(contribution.ContributionId ?? 0);

                return true;
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return false;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
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
            // TODO: CACHING
            try
            {
                return await _api.GetContributionTypes();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
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
            // TODO: CACHING
            try
            {
                return await _api.GetContributionAreas();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
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
            // TODO: CACHING
            try
            {
                return await _api.GetVisibilities();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Returns a list of the MVP's OnlineIdentities (social media accounts and other identities)
        /// </summary>
        /// <param name="forceRefresh">The result is cached in a backing list by default which prevents unnecessary fetches. If you want the cache refreshed, set this to true</param>
        /// <returns></returns>
        public async Task<IReadOnlyList<OnlineIdentity>> GetOnlineIdentitiesAsync(bool forceRefresh = false)
        {
            // TODO: CACHING
            try
            {
                return await _api.GetOnlineIdentities();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Saves an OnlineIdentity
        /// </summary>
        /// <param name="onlineIdentity"></param>
        /// <returns></returns>
        public async Task<OnlineIdentity> SubmitOnlineIdentityAsync(OnlineIdentity onlineIdentity)
        {
            if (onlineIdentity == null)
                throw new NullReferenceException($"The {nameof(onlineIdentity)} parameter was null.");

            try
            {
                return await _api.AddOnlineIdentity(onlineIdentity);
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                return null;
            }
        }

        public async Task<bool> DeleteOnlineIdentityAsync(OnlineIdentity onlineIdentity)
        {
            if (onlineIdentity == null)
                throw new NullReferenceException($"The {nameof(onlineIdentity)} parameter was null.");

            try
            {
                await _api.DeleteOnlineIdentity(onlineIdentity.PrivateSiteId ?? 0);
                return true;
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return false;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                return false;
            }
        }

        /// <summary>
        /// Gets the current Award Consideration Questions list.
        /// </summary>
        /// <returns>The list of questions to be answered for consideration in the next award period.</returns>
        public async Task<IReadOnlyList<AwardConsiderationQuestion>> GetAwardConsiderationQuestionsAsync()
        {
            try
            {
                return await _api.GetAwardConsiderationQuestions();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                return null;
            }
        }

        /// <summary>
        /// Gets the MVP's currently saved answers for the Award consideration questions
        /// </summary>
        /// <returns>The list of questions to be answered for consideration in the next award period.</returns>
        public async Task<IReadOnlyList<AwardConsiderationAnswer>> GetAwardConsiderationAnswersAsync()
        {
            try
            {
                return await _api.GetAwardConsiderationAnswers();
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                return null;
            }
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
                throw new NullReferenceException($"The {nameof(answers)} parameter was null.");

            try
            {
                return await _api.SaveAwardConsiderationAnswers(answers);
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return null;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                return null;
            }
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
                await _api.SubmitAwardConsiderationAnswers();
                return true;
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                return false;
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                return false;
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
            _analyticsService.Report(e);

            if (e.StatusCode == HttpStatusCode.InternalServerError)
            {
                HandleRequestErrorOccurred(string.Empty, isServerError: true);
            }
            else if (e.StatusCode == HttpStatusCode.Unauthorized || e.StatusCode == HttpStatusCode.Forbidden)
            {
                HandleAccessTokenExpired();
            }
            else if (e.StatusCode == HttpStatusCode.BadRequest)
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
    }
}