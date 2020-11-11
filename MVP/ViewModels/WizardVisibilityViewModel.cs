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
        Contribution contribution;

        public IAsyncCommand<Contribution> NextCommand { get; set; }
        public IAsyncCommand<VisibilityViewModel> SelectVisibilityCommand { get; }

        public IList<VisibilityViewModel> Visibilities { get; set; } = new List<VisibilityViewModel>();

        public WizardVisibilityViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            NextCommand = new AsyncCommand<Contribution>((contribution) => Next(contribution));
            SelectVisibilityCommand = new AsyncCommand<VisibilityViewModel>((x) => SelectVisibility(x));
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

        async Task SelectVisibility(VisibilityViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in Visibilities)
                item.IsSelected = false;

            vm.IsSelected = true;
            contribution.Visibility = vm.Visibility;
            await NavigationHelper.NavigateToAsync(nameof(WizardAmountsPage), contribution).ConfigureAwait(false);
        }

        async Task LoadVisibilities()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var result = await MvpApiService.GetVisibilitiesAsync().ConfigureAwait(false);

                if (result != null)
                {
                    Visibilities = result.Select(x => new VisibilityViewModel() { Visibility = x }).ToList();

                    // Editing mode
                    if (contribution.Visibility != null)
                    {
                        var selectedVisibility = Visibilities.FirstOrDefault(x => x.Visibility.Id == contribution.Visibility.Id);
                        selectedVisibility.IsSelected = true;
                    }
                }
            }
        }

        public async override Task Back()
        {
            contribution.Visibility = null;
            await NavigationHelper.BackAsync().ConfigureAwait(false);
        }

        async Task Next(Contribution contribution)
        {
            await NavigationHelper.NavigateToAsync(nameof(WizardAmountsPage), contribution).ConfigureAwait(false);
        }
    }
}
