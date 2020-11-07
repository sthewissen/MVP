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

        public IAsyncCommand NextCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }

        public IList<ContributionTechnology> SelectedContributionTechnologies { get; set; }
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

            SelectedContributionTechnologies = obj.Select(x => x as ContributionTechnology).ToList();
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

                        SelectedContributionTechnologies = result
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
            if (SelectedContributionTechnologies != null && SelectedContributionTechnologies.Any())
            {
                contribution.AdditionalTechnologies = new ObservableCollection<ContributionTechnology>(SelectedContributionTechnologies);
            }

            await NavigationHelper.NavigateToAsync(nameof(WizardDatePage), contribution).ConfigureAwait(false);
        }
    }
}
