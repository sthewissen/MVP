using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Services.Helpers;

namespace MVP.Services.Interfaces
{
    public interface IMvpApiService
    {
        Task ClearAllLocalData();

        Task<IReadOnlyList<ContributionCategory>> GetContributionAreasAsync(bool forceRefresh = false);
        Task<IReadOnlyList<ContributionType>> GetContributionTypesAsync(bool forceRefresh = false);
        Task<IReadOnlyList<Visibility>> GetVisibilitiesAsync(bool forceRefresh = false);

        Task<Profile> GetProfileAsync(bool forceRefresh = false);
        Task<string> GetProfileImageAsync(bool forceRefresh = false);

        Task<bool> DeleteOnlineIdentityAsync(OnlineIdentity onlineIdentity);
        Task<IReadOnlyList<OnlineIdentity>> GetOnlineIdentitiesAsync(bool forceRefresh = false);
        Task<OnlineIdentity> SubmitOnlineIdentityAsync(OnlineIdentity onlineIdentity);

        Task<IReadOnlyList<AwardConsiderationAnswer>> GetAwardConsiderationAnswersAsync();
        Task<IReadOnlyList<AwardConsiderationQuestion>> GetAwardConsiderationQuestionsAsync();
        Task<List<AwardConsiderationAnswer>> SaveAwardConsiderationAnswerAsync(IEnumerable<AwardConsiderationAnswer> answers);
        Task<bool> SubmitAwardConsiderationAnswerAsync();

        Task<bool> DeleteContributionAsync(Contribution contribution);
        Task<ContributionList> GetContributionsAsync(int offset = 0, int limit = 0, bool forceRefresh = false);
        Task<Contribution> SubmitContributionAsync(Contribution contribution);
        Task<bool> UpdateContributionAsync(Contribution contribution);

        event EventHandler<ApiServiceEventArgs> AccessTokenExpired;
        event EventHandler<ApiServiceEventArgs> RequestErrorOccurred;
    }
}