using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Extensions;
using MVP.Helpers;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using TinyMvvm;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class AdditionalTechnologyPickerViewModel : BaseViewModel
    {
        ContributionViewModel contribution;
        IList<ContributionTechnologyViewModel> selectedTechnologies = new List<ContributionTechnologyViewModel>();
        IReadOnlyList<Models.ContributionCategory> allCategories = new List<Models.ContributionCategory>();

        public string SearchText { get; set; } = string.Empty;

        public ICommand SelectContributionTechnologyCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public IAsyncCommand RefreshDataCommand { get; set; }

        public IList<Grouping<string, ContributionTechnologyViewModel>> GroupedContributionTechnologies { get; set; } = new List<Grouping<string, ContributionTechnologyViewModel>>();

        public AdditionalTechnologyPickerViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
            SearchCommand = new Command(() => PopulateList());
            RefreshDataCommand = new AsyncCommand(() => LoadContributionAreas(true));
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

        async Task LoadContributionAreas(bool force = false)
        {
            try
            {
                State = LayoutState.Loading;

                allCategories = await MvpApiService.GetContributionAreasAsync(force).ConfigureAwait(false);

                PopulateList();
            }
            finally
            {
                State = GroupedContributionTechnologies.Count > 0 ? LayoutState.None : LayoutState.Empty;
            }
        }

        void PopulateList()
        {
            if (allCategories == null)
                return;

            var result = new List<Grouping<string, ContributionTechnologyViewModel>>();

            foreach (var item in allCategories.SelectMany(x => x.ContributionAreas))
            {
                var data = item.ContributionTechnology
                                .Where(x => x.Name.ToLowerInvariant().Contains(SearchText.ToLowerInvariant()) || string.IsNullOrEmpty(SearchText))
                                .Select(x => new ContributionTechnologyViewModel() { ContributionTechnology = x });

                if (data.Any())
                    result.Add(new Grouping<string, ContributionTechnologyViewModel>(item.AwardName, data));
            }

            GroupedContributionTechnologies = result;

            // Editing mode
            if (contribution.AdditionalTechnologies == null || !contribution.AdditionalTechnologies.Any())
                return;

            var selectedValues = contribution.AdditionalTechnologies.Select(x => x.Id).ToList();

            selectedTechnologies = result
                .SelectMany(x => x)
                .Where(x => selectedValues.Contains(x.ContributionTechnology.Id))
                .ToList();

            foreach (var item in selectedTechnologies)
                item.IsSelected = true;
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

            //TODO: Replace by the back navigation version.
            contribution.AdditionalTechnologies = selectedTechnologies.Select(x => x.ContributionTechnology).ToList();
        }

        public async override Task Back()
            => await NavigationHelper.BackAsync(); // TODO: TinyMVVM 3.0 - selectedTechnologies.Select(x => x.ContributionTechnology).ToList());

    }
}
