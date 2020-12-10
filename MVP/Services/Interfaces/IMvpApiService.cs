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

        Task<bool> DeleteContributionAsync(Contribution contribution);
        Task<ContributionList> GetContributionsAsync(int offset = 0, int limit = 0, bool forceRefresh = false);
        Task<Contribution> SubmitContributionAsync(Contribution contribution);
        Task<bool> UpdateContributionAsync(Contribution contribution);

        event EventHandler<ApiServiceEventArgs> AccessTokenExpired;
        event EventHandler<ApiServiceEventArgs> RequestErrorOccurred;
    }
}