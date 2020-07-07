using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using MVP.Models;
using MVP.Pages;
using MVP.Services;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class WizardAdditionalTechnologyViewModel : BaseViewModel
    {
        IList<ContributionTechnology> selectedContributionTechnologies;
        Contribution contribution;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand NextCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }

        public IList<ContributionTechnology> SelectedContributionTechnologies
        {
            get { return selectedContributionTechnologies; }
            set { selectedContributionTechnologies = value; }
        }

        public IList<MvvmHelpers.Grouping<string, ContributionTechnology>> GroupedContributionTechnologies { get; set; } = new List<MvvmHelpers.Grouping<string, ContributionTechnology>>();

        public WizardAdditionalTechnologyViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand(() => Next());
            SelectionChangedCommand = new Command<IList<object>>((list) => SelectionChanged(list));
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

        void SelectionChanged(IList<object> obj)
        {
            if (obj.Count > 2)
            {
                obj.Remove(obj.FirstOrDefault());
            }

            selectedContributionTechnologies = obj.Select(x => x as ContributionTechnology).ToList();
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
                    if (contribution.AdditionalTechnologies != null && contribution.AdditionalTechnologies.Any())
                    {
                        var selectedValues = contribution.AdditionalTechnologies.Select(x => x.Id).ToList();

                        selectedContributionTechnologies = result
                            .SelectMany(x => x)
                            .Where(x => selectedValues.Contains(x.Id))
                            .ToList();

                        RaisePropertyChanged(nameof(SelectedContributionTechnologies));
                    }
                }
            }
        }

        async Task Back()
        {
            await NavigationHelper.BackAsync().ConfigureAwait(false);
        }

        async Task Next()
        {
            if (selectedContributionTechnologies != null && selectedContributionTechnologies.Any())
            {
                contribution.AdditionalTechnologies = new ObservableCollection<ContributionTechnology>(selectedContributionTechnologies);
            }

            await NavigationHelper.NavigateToAsync(nameof(WizardDatePage), contribution).ConfigureAwait(false);
        }
    }
}
