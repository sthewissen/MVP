using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using MvvmHelpers;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class WizardTechnologyViewModel : BaseViewModel
    {
        ContributionTechnology selectedContributionTechnology;
        Contribution contribution;

        public IAsyncCommand<Contribution> NextCommand { get; set; }

        public IList<MvvmHelpers.Grouping<string, ContributionTechnology>> GroupedContributionTechnologies { get; set; } = new List<MvvmHelpers.Grouping<string, ContributionTechnology>>();

        public ContributionTechnology SelectedContributionTechnology
        {
            get => selectedContributionTechnology;
            set
            {
                selectedContributionTechnology = value;

                if (value != null)
                {
                    contribution.ContributionTechnology = value;
                    NextCommand.Execute(contribution);
                }
            }
        }

        public WizardTechnologyViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand<Contribution>((contribution) => Next(contribution));
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contribution)
            {
                this.contribution = contribution;
            }

            LoadContributionAreas().SafeFireAndForget();
        }

        async Task LoadContributionAreas()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var categories = await MvpApiService.GetContributionAreasAsync().ConfigureAwait(false);

                if (categories != null)
                {
                    var result = new List<MvvmHelpers.Grouping<string, ContributionTechnology>>();

                    foreach (var item in categories.SelectMany(x => x.ContributionAreas))
                    {
                        result.Add(new MvvmHelpers.Grouping<string, ContributionTechnology>(item.AwardName, item.ContributionTechnology));
                    }

                    GroupedContributionTechnologies = result;

                    // Editing mode
                    if (contribution.ContributionTechnology != null)
                    {
                        selectedContributionTechnology = result
                            .SelectMany(x => x)
                            .FirstOrDefault(x => x.Id == contribution.ContributionTechnology.Id);

                        RaisePropertyChanged(nameof(SelectedContributionTechnology));
                    }
                }
            }
        }

        async Task Back()
        {
            if (contribution.ContributionId.HasValue)
            {
                // Pop the entire modal stack instead of just going back one screen.
                // This means it's editing mode and there is no way to go back and change activity type.
                await NavigationHelper.CloseModalAsync().ConfigureAwait(false);
            }
            else
            {
                await NavigationHelper.BackAsync();
            }
        }

        async Task Next(Contribution contribution)
        {
            await NavigationHelper.NavigateToAsync(nameof(WizardAdditionalTechnologyPage), contribution).ConfigureAwait(false);
        }
    }
}
