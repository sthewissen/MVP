using System.Collections.Generic;
using System.Threading.Tasks;
using MVP.Models;

namespace MVP.Services
{
    public interface IMvpApiService
    {
        Task<bool?> DeleteContributionAsync(Contribution contribution);
        Task<bool> DeleteOnlineIdentityAsync(OnlineIdentity onlineIdentity);
        Task<string> DownloadAndSaveProfileImage();
        Task<ContributionList> GetAllContributionsAsync(bool forceRefresh = false);
        Task<IReadOnlyList<AwardConsiderationAnswer>> GetAwardConsiderationAnswersAsync();
        Task<IReadOnlyList<AwardConsiderationQuestion>> GetAwardConsiderationQuestionsAsync();
        Task<IReadOnlyList<ContributionCategory>> GetContributionAreasAsync(bool forceRefresh = false);
        Task<ContributionList> GetContributionsAsync(int? offset, int limit, bool forceRefresh = false);
        Task<IReadOnlyList<ContributionType>> GetContributionTypesAsync(bool forceRefresh = false);
        Task<IReadOnlyList<OnlineIdentity>> GetOnlineIdentitiesAsync(bool forceRefresh = false);
        Task<Profile> GetProfileAsync();
        Task<string> GetProfileImageAsync();
        Task<IReadOnlyList<Visibility>> GetVisibilitiesAsync(bool forceRefresh = false);
        Task<List<AwardConsiderationAnswer>> SaveAwardConsiderationAnswerAsync(IEnumerable<AwardConsiderationAnswer> answers);
        Task<bool> SubmitAwardConsiderationAnswerAsync();
        Task<Contribution> SubmitContributionAsync(Contribution contribution);
        Task<OnlineIdentity> SubmitOnlineIdentityAsync(OnlineIdentity onlineIdentity);
        Task<bool?> UpdateContributionAsync(Contribution contribution);
    }
}