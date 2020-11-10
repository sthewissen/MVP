using System;
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
    public class WizardContributionTypeViewModel : BaseViewModel
    {
        Contribution contribution = new Contribution();

        public IAsyncCommand<ContributionTypeViewModel> SelectContributionTypeCommand { get; set; }

        public List<ContributionTypeViewModel> ContributionTypes { get; set; } = new List<ContributionTypeViewModel>();

        public WizardContributionTypeViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            SelectContributionTypeCommand = new AsyncCommand<ContributionTypeViewModel>((x) => SelectionContributionType(x));
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
                    ContributionTypes = types
                        .OrderBy(x => x.Name)
                        .Select(x => new ContributionTypeViewModel
                        {
                            ContributionType = x
                        }).ToList();
                }
            }
        }

        async Task SelectionContributionType(ContributionTypeViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in ContributionTypes)
                item.IsSelected = false;

            vm.IsSelected = true;
            contribution.ContributionType = vm.ContributionType;
            await NavigationHelper.NavigateToAsync(nameof(WizardTechnologyPage), contribution).ConfigureAwait(false);
        }

        public async override Task Back()
        {
            // Pop the entire modal stack instead of just going back one screen.
            await NavigationHelper.CloseModalAsync().ConfigureAwait(false);
        }
    }
}
