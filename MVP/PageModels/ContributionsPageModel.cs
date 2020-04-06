using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Extensions;
using MVP.Models;
using MVP.Services;
using MvvmHelpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.PageModels
{
    public class ContributionsPageModel : BasePageModel
    {
        public IList<Grouping<int, Contribution>> Contributions { get; set; }
        public Profile Profile { get; set; }
        public ImageSource ProfileImage { get; set; }

        public IAsyncCommand OpenProfileCommand { get; set; }
        public IAsyncCommand RefreshDataCommand { get; set; }
        public IAsyncCommand<Contribution> OpenContributionCommand { get; set; }
        public IAsyncCommand OpenAddContributionCommand { get; set; }

        public bool IsRefreshing { get; set; }

        public ContributionsPageModel()
        {
            OpenProfileCommand = new AsyncCommand(OpenProfile);
            OpenContributionCommand = new AsyncCommand<Contribution>(OpenContribution);
            OpenAddContributionCommand = new AsyncCommand(OpenAddContribution);
            RefreshDataCommand = new AsyncCommand(RefreshData);
        }

        async Task RefreshData()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet && App.MvpApiService != null)
            {
                var contributionsList = await App.MvpApiService.GetAllContributionsAsync().ConfigureAwait(false);

                if (contributionsList != null)
                {
                    Contributions = contributionsList.ToGroupedContributions();
                }

                var image = await App.MvpApiService.GetProfileImageAsync();

                if (image != null)
                {
                    var imageStream = new MemoryStream(image);
                    ProfileImage = ImageSource.FromStream(() => imageStream);
                }

                IsRefreshing = false;
            }
        }

        async Task OpenProfile()
        {
            await CoreMethods.PushPageModel<ProfilePageModel>();
        }

        async Task OpenAddContribution()
        {
            await CoreMethods.PushPageModel<ProfilePageModel>();
        }

        async Task OpenContribution(Contribution contribution)
        {
            await CoreMethods.PushPageModel<ProfilePageModel>();
        }
    }
}
