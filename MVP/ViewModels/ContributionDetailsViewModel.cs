using System;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Models;
using MVP.Pages;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
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

        public ContributionDetailsViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
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

        /// <summary>
        /// Opens the edit contribution form.
        /// </summary>
        async Task EditContribution()
        {
            // Shouldn't be getting here anyway, so no need for a message.
            if (!CanBeEdited)
                return;

            await NavigationHelper.OpenModalAsync(nameof(ContributionFormPage), Contribution, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a contribution.
        /// </summary>
        async Task DeleteContribution()
        {
            try
            {
                // Shouldn't be getting here anyway, so no need for a message.
                if (!CanBeEdited)
                    return;

                if (!await VerifyInternetConnection())
                    return;

                // Ask for confirmation before deletion.
                var confirm = await DialogService.ConfirmAsync(
                    Translations.alert_contribution_deleteconfirmation,
                    Translations.alert_warning_title,
                    Translations.alert_ok,
                    Translations.alert_cancel).ConfigureAwait(false);

                if (!confirm)
                    return;

                var isDeleted = await MvpApiService.DeleteContributionAsync(Contribution);

                if (isDeleted)
                {
                    // TODO: Pass back true to indicate it needs to refresh.
                    // TODO: Be a bit more sensible with muh threads plz.
                    MainThread.BeginInvokeOnMainThread(() => HapticFeedback.Perform(HapticFeedbackType.LongPress));
                    AnalyticsService.Track("Contribution Deleted");
                    await MainThread.InvokeOnMainThreadAsync(() => NavigationHelper.BackAsync());
                }
                else
                {
                    await DialogService.AlertAsync(
                        Translations.alert_contribution_notdeleted,
                        Translations.alert_error_title,
                        Translations.alert_ok).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex);

                await DialogService.AlertAsync(
                    Translations.alert_error_unexpected,
                    Translations.alert_error_title,
                    Translations.alert_ok).ConfigureAwait(false);
            }
        }

        async Task OpenUrl()
            => await Browser.OpenAsync(Contribution.ReferenceUrl, new BrowserLaunchOptions { Flags = BrowserLaunchFlags.PresentAsPageSheet }).ConfigureAwait(false);
    }
}
