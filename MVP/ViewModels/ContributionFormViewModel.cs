using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using TinyMvvm;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class ContributionFormViewModel : BaseViewModel
    {
        public bool IsEditing { get; set; }
        public ContributionViewModel Contribution { get; set; } = new ContributionViewModel();

        public ContributionTypeConfig ContributionTypeConfig { get; set; }

        public IAsyncCommand PickAdditionalTechnologiesCommand { get; set; }
        public IAsyncCommand PickContributionTypeCommand { get; set; }
        public IAsyncCommand PickVisibilityCommand { get; set; }
        public IAsyncCommand PickContributionTechnologyCommand { get; set; }

        public ContributionFormViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            PickAdditionalTechnologiesCommand = new AsyncCommand(PickAdditionalTechnologies);
            PickContributionTypeCommand = new AsyncCommand(PickContributionType);
            PickVisibilityCommand = new AsyncCommand(PickVisibility);
            PickContributionTechnologyCommand = new AsyncCommand(PickContributionTechnology);
            SecondaryCommand = new Command(() => { if (Contribution.IsValid()) return; });
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contribution)
            {
                Contribution = contribution.ToContributionViewModel();

                IsEditing = contribution.ContributionId.HasValue && contribution.ContributionId.Value > 0;

                if (contribution.ContributionType != null && contribution.ContributionType.Id.HasValue)
                {
                    ContributionTypeConfig = contribution.ContributionType.Id.Value.GetContributionTypeRequirements();
                }
            }

            Contribution.AddValidationRules();
        }

        public override async Task Returning()
        {
            await base.Returning();

            if (ReturningParameter is ContributionType type)
            {
                Contribution.ContributionType.Value = type;
                ContributionTypeConfig = type.Id.Value.GetContributionTypeRequirements();
            }
            else if (ReturningParameter is Visibility vis)
            {
                Contribution.Visibility.Value = vis;
            }
            else if (ReturningParameter is IList<ContributionTechnology> techs)
            {
                Contribution.AdditionalTechnologies = techs;
            }
            else if (ReturningParameter is ContributionTechnology tech)
            {
                Contribution.ContributionTechnology.Value = tech;
            }
        }

        // Pop the entire modal stack instead of just going back one screen.
        // This means it's editing mode and there is no way to go back and change activity type.
        public async override Task Back()
            => await NavigationHelper.CloseModalAsync().ConfigureAwait(false);

        async Task PickAdditionalTechnologies()
            => await NavigationHelper.NavigateToAsync(nameof(AdditionalTechnologyPickerPage), Contribution).ConfigureAwait(false);

        async Task PickContributionTechnology()
            => await NavigationHelper.NavigateToAsync(nameof(ContributionTechnologyPickerPage), Contribution).ConfigureAwait(false);

        async Task PickVisibility()
            => await NavigationHelper.NavigateToAsync(nameof(VisibilityPickerPage), Contribution).ConfigureAwait(false);

        async Task PickContributionType()
            => await NavigationHelper.NavigateToAsync(nameof(ContributionTypePickerPage), Contribution).ConfigureAwait(false);
    }
}
