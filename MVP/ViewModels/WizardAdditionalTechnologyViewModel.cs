using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using MvvmHelpers;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class WizardAdditionalTechnologyViewModel : BaseViewModel
    {
        Contribution contribution;
        IList<ContributionTechnologyViewModel> selectedTechnologies = new List<ContributionTechnologyViewModel>();

        public IAsyncCommand NextCommand { get; set; }
        public ICommand SelectContributionTechnologyCommand { get; set; }

        public IList<Grouping<string, ContributionTechnologyViewModel>> GroupedContributionTechnologies { get; set; } = new List<Grouping<string, ContributionTechnologyViewModel>>();

        public WizardAdditionalTechnologyViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            NextCommand = new AsyncCommand(() => Next());
            SelectContributionTechnologyCommand = new Command<ContributionTechnologyViewModel>((x) => SelectContributionTechnology(x));
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

        void SelectContributionTechnology(ContributionTechnologyViewModel vm)
        {
            if (vm.IsSelected)
            {
                selectedTechnologies.Remove(vm);
                vm.IsSelected = false;
                return;
            }

            // Max two allowed. Remove first from the selection if another is added.
            if (selectedTechnologies.Count == 2)
            {
                selectedTechnologies[0].IsSelected = false;
                selectedTechnologies.RemoveAt(0);
            }

            selectedTechnologies.Add(vm);
            vm.IsSelected = true;
        }

        public async override Task Back()
        {
            contribution.AdditionalTechnologies = null;
            await NavigationHelper.BackAsync();
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
                        result.Add(new Grouping<string, ContributionTechnologyViewModel>(item.AwardName,
                            item.ContributionTechnology.Select(x => new ContributionTechnologyViewModel() { ContributionTechnology = x })));

                    }

                    GroupedContributionTechnologies = result;

                    // Editing mode
                    if (contribution.AdditionalTechnologies != null && contribution.AdditionalTechnologies.Any())
                    {
                        var selectedValues = contribution.AdditionalTechnologies.Select(x => x.Id).ToList();

                        selectedTechnologies = result
                            .SelectMany(x => x)
                            .Where(x => selectedValues.Contains(x.ContributionTechnology.Id))
                            .ToList();

                        foreach (var item in selectedTechnologies)
                            item.IsSelected = true;
                    }
                }
            }
        }

        async Task Next()
        {
            // If additional tech is selected, add it to our root object.
            if (selectedTechnologies.Any())
                contribution.AdditionalTechnologies = new ObservableCollection<ContributionTechnology>(selectedTechnologies.Select(x => x.ContributionTechnology));

            await NavigationHelper.NavigateToAsync(nameof(WizardDatePage), contribution).ConfigureAwait(false);
        }
    }
}
