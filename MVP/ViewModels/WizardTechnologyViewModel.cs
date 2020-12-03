using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using MvvmHelpers;
using TinyMvvm;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class WizardTechnologyViewModel : BaseViewModel
    {
        Contribution contribution;

        public bool IsEditing { get; set; }
        public IAsyncCommand NextCommand { get; set; }
        public IAsyncCommand<ContributionTechnologyViewModel> SelectContributionTechnologyCommand { get; set; }

        public IList<Grouping<string, ContributionTechnologyViewModel>> GroupedContributionTechnologies { get; set; } = new List<Grouping<string, ContributionTechnologyViewModel>>();

        public WizardTechnologyViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            NextCommand = new AsyncCommand(() => Next());
            SelectContributionTechnologyCommand = new AsyncCommand<ContributionTechnologyViewModel>((x) => SelectContributionTechnology(x));
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contribution)
            {
                this.contribution = contribution;
                IsEditing = contribution.ContributionId.HasValue && contribution.ContributionId.Value > 0;
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
                    var result = new List<Grouping<string, ContributionTechnologyViewModel>>();

                    foreach (var item in categories.SelectMany(x => x.ContributionAreas))
                    {
                        result.Add(
                            new Grouping<string, ContributionTechnologyViewModel>(item.AwardName,
                            item.ContributionTechnology.Select(x => new ContributionTechnologyViewModel()
                            {
                                ContributionTechnology = x
                            }))
                        );
                    }

                    GroupedContributionTechnologies = result;

                    // Editing mode
                    if (contribution.ContributionTechnology != null)
                    {
                        var selected = result
                            .SelectMany(x => x)
                            .FirstOrDefault(x => x.ContributionTechnology.Id == contribution.ContributionTechnology.Id);

                        selected.IsSelected = true;
                    }
                }
            }
        }

        public async override Task Back()
        {
            if (contribution.ContributionId.HasValue)
            {
                // Pop the entire modal stack instead of just going back one screen.
                // This means it's editing mode and there is no way to go back and change activity type.
                await NavigationHelper.CloseModalAsync().ConfigureAwait(false);
            }
            else
            {
                contribution.ContributionTechnology = null;
                await NavigationHelper.BackAsync();
            }
        }

        async Task SelectContributionTechnology(ContributionTechnologyViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in GroupedContributionTechnologies)
                foreach (var tech in item)
                    tech.IsSelected = false;

            vm.IsSelected = true;
            contribution.ContributionTechnology = vm.ContributionTechnology;
            //await NavigationHelper.NavigateToAsync(nameof(WizardAdditionalTechnologyPage), contribution).ConfigureAwait(false);
        }

        async Task Next()
        {
            //await NavigationHelper.NavigateToAsync(nameof(WizardAdditionalTechnologyPage), contribution).ConfigureAwait(false);
        }
    }
}
