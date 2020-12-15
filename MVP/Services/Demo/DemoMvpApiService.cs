using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Services.Helpers;
using MVP.Services.Interfaces;
using Newtonsoft.Json;

namespace MVP.Services.Demo
{
    public class DemoMvpApiService : IMvpApiService
    {
        static ContributionList allContributionList = null;

        public Task ClearAllLocalData()
            => Task.FromResult<object>(null);

        public async Task<Profile> GetProfileAsync(bool forceRefresh = false)
        {
            // Let's fake some delay, to see all the fancy loaders!
            await Task.Delay(1000);

            return JsonConvert.DeserializeObject<Profile>(LocalResourceService.GetFile("getprofile"));
        }

        public async Task<string> GetProfileImageAsync(bool forceRefresh = false)
        {
            // Let's fake some delay, to see all the fancy loaders!
            await Task.Delay(1000);

            var image = LocalResourceService.GetFile("getprofileimage");
            image = image.TrimStart('"').TrimEnd('"');

            return image; // $"data:image/png;base64,{image}";
        }

        public async Task<ContributionList> GetContributionsAsync(int offset = 0, int limit = 0, bool forceRefresh = false)
        {
            // Let's fake some delay, to see all the fancy loaders!
            await Task.Delay(3000);

            // Take the local copy, which we sort of cached in this service.
            if (!forceRefresh && allContributionList != null)
                return new ContributionList()
                {
                    Contributions = allContributionList.Contributions.Skip(offset).Take(limit == 0 ? allContributionList.Contributions.Count : limit).ToList(),
                    TotalContributions = allContributionList.Contributions.Count,
                    PagingIndex = 0
                };

            // Get them from "remote" aka reset the whole thing.
            var list = JsonConvert.DeserializeObject<List<Contribution>>(LocalResourceService.GetFile("getcontributions"));

            allContributionList = new ContributionList()
            {
                Contributions = list,
                TotalContributions = list.Count,
                PagingIndex = 0
            };

            return new ContributionList()
            {
                Contributions = list.Skip(offset).Take(limit == 0 ? allContributionList.Contributions.Count : limit).ToList(),
                TotalContributions = list.Count,
                PagingIndex = 0
            }; ;
        }

        public async Task<Contribution> SubmitContributionAsync(Contribution contribution)
        {
            // Let's fake some delay, to see all the fancy loaders!
            await Task.Delay(2000);

            contribution.ContributionId = allContributionList.Contributions.Max(x => x.ContributionId) + 1;
            allContributionList.Contributions.Add(contribution);
            allContributionList.TotalContributions += 1;

            return contribution;
        }

        public async Task<bool> UpdateContributionAsync(Contribution contribution)
        {
            // Let's fake some delay, to see all the fancy loaders!
            await Task.Delay(2000);

            var contribToDelete = allContributionList.Contributions.FirstOrDefault(x => x.ContributionId == contribution.ContributionId);

            if (contribToDelete == null)
                return false;

            allContributionList.Contributions.Remove(contribToDelete);
            allContributionList.Contributions.Add(contribution);

            return true;
        }

        public async Task<bool> DeleteContributionAsync(Contribution contribution)
        {
            // Let's fake some delay, to see all the fancy loaders!
            await Task.Delay(2000);

            var contribToDelete = allContributionList.Contributions.FirstOrDefault(x => x.ContributionId == contribution.ContributionId);

            if (contribToDelete == null)
                return false;

            allContributionList.Contributions.Remove(contribToDelete);

            return true;
        }

        public async Task<IReadOnlyList<ContributionType>> GetContributionTypesAsync(bool forceRefresh = false)
        {
            // Let's fake some delay, to see all the fancy loaders!
            await Task.Delay(1000);

            return JsonConvert.DeserializeObject<List<ContributionType>>(LocalResourceService.GetFile("getcontributiontypes"));
        }

        public async Task<IReadOnlyList<ContributionCategory>> GetContributionAreasAsync(bool forceRefresh = false)
        {
            // Let's fake some delay, to see all the fancy loaders!
            await Task.Delay(1000);

            return JsonConvert.DeserializeObject<List<ContributionCategory>>(LocalResourceService.GetFile("getcontributionareas"));
        }

        public async Task<IReadOnlyList<Visibility>> GetVisibilitiesAsync(bool forceRefresh = false)
        {
            // Let's fake some delay, to see all the fancy loaders!
            await Task.Delay(1000);

            return JsonConvert.DeserializeObject<List<Visibility>>(LocalResourceService.GetFile("getvisibilities"));
        }

        public event EventHandler<ApiServiceEventArgs> AccessTokenExpired;

        public event EventHandler<ApiServiceEventArgs> RequestErrorOccurred;
    }
}