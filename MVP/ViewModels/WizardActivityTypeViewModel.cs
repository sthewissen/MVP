using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class WizardActivityTypeViewModel : BaseViewModel
    {
        ContributionType selectedContribution;
        Contribution contribution = new Contribution();

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand NextCommand { get; set; }

        public List<ContributionType> ContributionTypes { get; set; } = new List<ContributionType>();

        public ContributionType SelectedContributionType
        {
            get => selectedContribution;
            set
            {
                selectedContribution = value;

                if (value != null)
                {
                    contribution.ContributionType = value;
                    NextCommand.Execute(contribution);
                }
            }
        }

        public WizardActivityTypeViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand(() => Next());
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            // If a new contribution is coming in, the user created one from the URL
            // they had on the clipboard.
            if (NavigationParameter is Contribution contribution)
            {
                this.contribution = contribution;
            }

            LoadContributionTypes().SafeFireAndForget();
        }

        async Task LoadContributionTypes()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var types = await MvpApiService.GetContributionTypesAsync().ConfigureAwait(false);

                if (types != null)
                {
                    ContributionTypes = types.OrderBy(x => x.Name).ToList();
                }
            }
        }

        async Task Back()
        {
            // Pop the entire modal stack instead of just going back one screen.
            await NavigationHelper.CloseModalAsync().ConfigureAwait(false);
        }

        async Task Next()
        {
            await NavigationHelper.NavigateToAsync(nameof(WizardTechnologyPage), contribution).ConfigureAwait(false);
        }
    }
}
