using System;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class ContributionDetailsViewModel : BaseViewModel
    {
        public Contribution Contribution { get; set; }
        public bool CanBeEdited => Contribution != null && Contribution.StartDate.IsWithinCurrentAwardPeriod();

        public IAsyncCommand DeleteContributionCommand { get; set; }
        public ContributionTypeConfig ContributionTypeConfig { get; set; }

        public ContributionDetailsViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            DeleteContributionCommand = new AsyncCommand(() => DeleteContribution());
            SecondaryCommand = new AsyncCommand(() => EditContribution(), (x) => CanBeEdited);
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contribution)
            {
                Contribution = contribution;

                RaisePropertyChanged(nameof(CanBeEdited));
                ((AsyncCommand)SecondaryCommand).RaiseCanExecuteChanged();

                if (contribution.ContributionType.Id.HasValue)
                {
                    ContributionTypeConfig = contribution.ContributionType.Id.Value.GetContributionTypeRequirements();
                }
            }
        }

        async Task EditContribution()
        {
            // TODO: Change this check to block people if current time is between April 1 and July 1 and the
            // contribution is before April 1st.
            if (Contribution.StartDate.IsWithinCurrentAwardPeriod())
            {
                await NavigationHelper.OpenModalAsync(nameof(WizardTechnologyPage), Contribution, true).ConfigureAwait(false);
            }
            else
            {
                // TODO: Message
            }
        }

        async Task DeleteContribution()
        {
            try
            {
                // Shouldn't be getting here anyway, so no need for a message.
                if (!Contribution.StartDate.IsWithinCurrentAwardPeriod())
                    return;

                // Ask for confirmation before deletion.
                var confirm = await DialogService.ConfirmAsync("Are you sure you want to delete this contribution? You cannot undo this.", Alerts.HoldOn, Alerts.OK, Alerts.Cancel);

                if (!confirm)
                    return;

                var isDeleted = await MvpApiService.DeleteContributionAsync(Contribution);

                if (isDeleted)
                {
                    // TODO: Pass back true to indicate it needs to refresh.
                    await NavigationHelper.BackAsync().ConfigureAwait(false);
                }
                else
                {
                    await DialogService.AlertAsync("Your contribution could not be deleted. Perhaps it was already deleted, or it took place in the previous award period?", Alerts.Error, Alerts.OK);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex);
                await DialogService.AlertAsync(Alerts.UnexpectedError, Alerts.Error, Alerts.OK).ConfigureAwait(false);
            }
        }
    }
}
