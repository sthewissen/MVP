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
    public class WizardVisibilityViewModel : BaseViewModel
    {
        Visibility selectedVisibility;
        Contribution contribution;

        public IAsyncCommand<Contribution> NextCommand { get; set; }

        public IList<Visibility> Visibilities { get; set; } = new List<Visibility>();

        public Visibility SelectedVisibility
        {
            get => selectedVisibility;
            set
            {
                selectedVisibility = value;

                if (value != null && contribution != null)
                {
                    contribution.Visibility = value;
                    NextCommand.Execute(contribution);
                }
            }
        }

        public WizardVisibilityViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand<Contribution>((contribution) => Next(contribution));
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contrib)
            {
                contribution = contrib;
            }

            LoadVisibilities().SafeFireAndForget();
        }

        async Task LoadVisibilities()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var result = await MvpApiService.GetVisibilitiesAsync().ConfigureAwait(false);

                if (result != null)
                {
                    Visibilities = result.ToList();

                    // Editing mode
                    if (contribution.Visibility != null)
                    {
                        selectedVisibility = result
                            .FirstOrDefault(x => x.Id == contribution.Visibility.Id);

                        RaisePropertyChanged(nameof(SelectedVisibility));
                    }
                }
            }
        }

        async Task Back()
        {
            await NavigationHelper.BackAsync().ConfigureAwait(false);
        }

        async Task Next(Contribution contribution)
        {
            await NavigationHelper.NavigateToAsync(nameof(WizardAmountsPage), contribution).ConfigureAwait(false);
        }
    }
}
