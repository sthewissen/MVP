using System;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyMvvm;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class ContributionDetailsViewModel : BaseViewModel
    {
        public Contribution Contribution { get; set; }
        public bool CanBeEdited => Contribution != null && Contribution.StartDate.IsWithinCurrentAwardPeriod();

        public IAsyncCommand DeleteContributionCommand { get; set; }
        public IAsyncCommand OpenUrlCommand { get; set; }
        public ContributionTypeConfig ContributionTypeConfig { get; set; }

        public ContributionDetailsViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
            DeleteContributionCommand = new AsyncCommand(() => DeleteContribution());
            SecondaryCommand = new AsyncCommand(() => EditContribution(), (x) => CanBeEdited);
            OpenUrlCommand = new AsyncCommand(() => OpenUrl());
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
            // Shouldn't be getting here anyway, so no need for a message.
            if (!CanBeEdited)
                return;

            await NavigationHelper.OpenModalAsync(nameof(ContributionFormPage), Contribution, true).ConfigureAwait(false);
        }

        async Task DeleteContribution()
        {
            try
            {
                // Shouldn't be getting here anyway, so no need for a message.
                if (!CanBeEdited)
                    return;

                // Ask for confirmation before deletion.
                var confirm = await DialogService.ConfirmAsync(
                    Resources.Translations.alert_contribution_deleteconfirmation,
                    Resources.Translations.alert_warning_title,
                    Resources.Translations.alert_ok,
                    Resources.Translations.alert_cancel
                ).ConfigureAwait(false);

                if (!confirm)
                    return;

                var isDeleted = await MvpApiService.DeleteContributionAsync(Contribution);

                if (isDeleted)
                {
                    // TODO: Pass back true to indicate it needs to refresh.
                    // TODO: Be a bit more sensible with muh threads plz.
                    MainThread.BeginInvokeOnMainThread(() => HapticFeedback.Perform(HapticFeedbackType.LongPress));
                    await MainThread.InvokeOnMainThreadAsync(() => NavigationHelper.BackAsync());
                }
                else
                {
                    await DialogService.AlertAsync(
                        Resources.Translations.alert_contribution_notdeleted,
                        Resources.Translations.alert_error_title,
                        Resources.Translations.alert_ok
                    ).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex);

                await DialogService.AlertAsync(
                    Resources.Translations.alert_error_title,
                    Resources.Translations.alert_error_unexpected,
                    Resources.Translations.alert_ok
                ).ConfigureAwait(false);
            }
        }

        async Task OpenUrl()
            => await Browser.OpenAsync(Contribution.ReferenceUrl);
    }
}
