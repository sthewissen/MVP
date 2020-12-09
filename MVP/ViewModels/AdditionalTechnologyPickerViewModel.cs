using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using MvvmHelpers;
using TinyMvvm;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class AdditionalTechnologyPickerViewModel : BaseViewModel
    {
        ContributionViewModel contribution;
        IList<ContributionTechnologyViewModel> selectedTechnologies = new List<ContributionTechnologyViewModel>();

        public ICommand SelectContributionTechnologyCommand { get; set; }

        public IList<Grouping<string, ContributionTechnologyViewModel>> GroupedContributionTechnologies { get; set; } = new List<Grouping<string, ContributionTechnologyViewModel>>();

        public AdditionalTechnologyPickerViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            SelectContributionTechnologyCommand = new Command<ContributionTechnologyViewModel>((x) => SelectContributionTechnology(x));
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is ContributionViewModel contribution)
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
            => await NavigationHelper.BackAsync(selectedTechnologies.Select(x => x.ContributionTechnology).ToList());

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
    }
}
