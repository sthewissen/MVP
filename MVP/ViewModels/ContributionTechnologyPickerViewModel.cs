using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Extensions;
using MVP.Helpers;
using MVP.Services;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class ContributionTechnologyPickerViewModel : BaseViewModel
    {
        ContributionViewModel contribution;
        IReadOnlyList<Models.ContributionCategory> allCategories = new List<Models.ContributionCategory>();

        public string SearchText { get; set; } = string.Empty;

        public IAsyncCommand<ContributionTechnologyViewModel> SelectContributionTechnologyCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public IAsyncCommand RefreshDataCommand { get; set; }

        public IList<Grouping<string, ContributionTechnologyViewModel>> GroupedContributionTechnologies { get; set; } = new List<Grouping<string, ContributionTechnologyViewModel>>();
        public List<ContributionTechnologyViewModel> Suggestions { get; set; } = new List<ContributionTechnologyViewModel>();

        public ContributionTechnologyPickerViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            SearchCommand = new Command(() => PopulateList());
            RefreshDataCommand = new AsyncCommand(() => LoadContributionAreas(true));
            SelectContributionTechnologyCommand = new AsyncCommand<ContributionTechnologyViewModel>((x) => SelectContributionTechnology(x));
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

        /// <summary>
        /// Loads the contribution areas for the contribution.
        /// </summary>
        async Task LoadContributionAreas(bool force = false)
        {
            try
            {
                State = LayoutState.Loading;

                allCategories = await MvpApiService.GetContributionAreasAsync(force).ConfigureAwait(false);

                if (allCategories == null)
                {
                    State = LayoutState.Error;
                    return;
                }

                PopulateList();

                // Gather suggestions
                var suggestions = await SuggestionService.GetContributionTechnologySuggestions();
                var items = allCategories
                    .SelectMany(x => x.ContributionAreas)
                    .SelectMany(y => y.ContributionTechnology)
                    .Where(x => suggestions.Contains(x.Id ?? Guid.Empty));

                Suggestions = new List<ContributionTechnologyViewModel>(items
                    .Select(x => new ContributionTechnologyViewModel { ContributionTechnology = x }));
            }
            catch (Exception ex)
            {
                State = LayoutState.Error;
                AnalyticsService.Report(ex);
            }
            finally
            {
                if (State != LayoutState.Error)
                    State = GroupedContributionTechnologies.Count > 0 ? LayoutState.None : LayoutState.Empty;
            }
        }

        /// <summary>
        /// Populates the list of contribution areas based on search.
        /// </summary>
        void PopulateList()
        {
            try
            {
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
                if (contribution.ContributionTechnology.Value == null)
                    return;

                var selected = result
                    .SelectMany(x => x)
                    .FirstOrDefault(x => x.ContributionTechnology.Id == contribution.ContributionTechnology.Value.Id);

                if (selected != null)
                    selected.IsSelected = true;
            }
            catch (Exception ex)
            {
                State = LayoutState.Error;
                AnalyticsService.Report(ex);
            }
        }

        /// <summary>
        /// Selects a contribution area for this contribution.
        /// </summary>
        async Task SelectContributionTechnology(ContributionTechnologyViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in GroupedContributionTechnologies)
                foreach (var tech in item)
                    tech.IsSelected = false;

            vm.IsSelected = true;

            //TODO: Replace by the back navigation version.
            contribution.ContributionTechnology.Value = vm.ContributionTechnology;

            AnalyticsService.Track("Contribution Technology Picked",
                nameof(contribution.ContributionTechnology),
                vm.ContributionTechnology.Name);

            await BackAsync();
        }
    }
}
