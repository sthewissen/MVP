using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using FormsToolkit;
using MVP.Extensions;
using MVP.Models;
using MVP.Services;
using MvvmHelpers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.StateSquid;

namespace MVP.PageModels
{
    public class ContributionsPageModel : BasePageModel
    {
        readonly MvpApiService _mvpApiService;

        public IList<Grouping<int, Contribution>> Contributions { get; set; }
        public Profile Profile { get; set; }
        public string ProfileImage { get; set; }

        public IAsyncCommand OpenProfileCommand { get; set; }
        public IAsyncCommand RefreshDataCommand { get; set; }
        public IAsyncCommand<Contribution> OpenContributionCommand { get; set; }
        public IAsyncCommand HackTokenCommand { get; set; }
        public IAsyncCommand OpenAddContributionCommand { get; set; }

        public string Name { get; set; }

        public ContributionsPageModel(MvpApiService mvpApiService)
        {
            _mvpApiService = mvpApiService;

            OpenProfileCommand = new AsyncCommand(OpenProfile);
            OpenContributionCommand = new AsyncCommand<Contribution>(OpenContribution);
            OpenAddContributionCommand = new AsyncCommand(OpenAddContribution);
            RefreshDataCommand = new AsyncCommand(RefreshContributions);
        }

        public async override void Init(object initData)
        {
            base.Init(initData);

            await RefreshData().ConfigureAwait(false);
        }

        async Task RefreshData()
        {
            await Task.WhenAll(RefreshContributions(), RefreshProfileData()).ConfigureAwait(false);
        }

        async Task RefreshContributions()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var contributionsList = await _mvpApiService.GetContributionsAsync(0, 10, true).ConfigureAwait(false);

                if (contributionsList != null)
                {
                    Contributions = contributionsList.ToGroupedContributions();
                }
            }
        }

        async Task RefreshProfileData()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var profile = await _mvpApiService.GetProfileAsync().ConfigureAwait(false);
                var image = await _mvpApiService.GetProfileImageAsync().ConfigureAwait(false);

                Name = profile.FullName;
                ProfileImage = image;
            }
        }

        async Task OpenProfile()
        {
            await CoreMethods.PushPageModel<ProfilePageModel>().ConfigureAwait(false);
        }

        async Task OpenAddContribution()
        {
            await CoreMethods.PushPageModel<ProfilePageModel>().ConfigureAwait(false);
        }

        async Task OpenContribution(Contribution contribution)
        {
            await CoreMethods.PushPageModel<ContributionDetailsPageModel>(contribution).ConfigureAwait(false);
        }
    }
}
