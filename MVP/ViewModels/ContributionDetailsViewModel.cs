using System;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Models;
using MVP.Pages;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class ContributionDetailsViewModel : BaseViewModel
    {
        public Contribution Contribution { get; set; }
        public bool CanBeEdited { get; set; } = true;

        public IAsyncCommand DeleteContributionCommand { get; set; }
        public IAsyncCommand OpenUrlCommand { get; set; }
        public ContributionTypeConfig ContributionTypeConfig { get; set; }

        public ContributionDetailsViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            DeleteContributionCommand = new AsyncCommand(() => DeleteContribution());
            SecondaryCommand = new AsyncCommand(() => EditContribution(), (x) => CanBeEdited);
            OpenUrlCommand = new AsyncCommand(() => OpenUrl());

            MessagingService.Current.Subscribe<Contribution>(MessageKeys.InMemoryUpdate, HandleInMemoryUpdateMessage);
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contribution)
            {
                Contribution = contribution;
                CanBeEdited = Contribution.IsEditable;

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

            await OpenModalAsync(nameof(ContributionFormPage), Contribution, true).ConfigureAwait(false);
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
                var confirm = await DialogService.ConfirmAsync(Translations.contributiondetail_deleteconfirmation, Translations.warning_title, Translations.ok, Translations.cancel).ConfigureAwait(false);

                if (!confirm)
                    return;

                State = LayoutState.Loading;

                var isDeleted = await MvpApiService.DeleteContributionAsync(Contribution);

                if (isDeleted)
                {
                    // TODO: Pass back true to indicate it needs to refresh.
                    // TODO: Be a bit more sensible with muh threads plz.
                    MainThread.BeginInvokeOnMainThread(() => HapticFeedback.Perform(HapticFeedbackType.LongPress));
                    AnalyticsService.Track("Contribution Deleted");
                    await MainThread.InvokeOnMainThreadAsync(() => BackAsync());
                    MessagingService.Current.SendMessage(MessageKeys.RefreshNeeded);
                }
                else
                {
                    await DialogService.AlertAsync(Translations.contributiondetail_notdeleted, Translations.error_title, Translations.ok).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex);
                await DialogService.AlertAsync(Translations.error_unexpected, Translations.error_title, Translations.ok).ConfigureAwait(false);
            }
            finally
            {
                State = LayoutState.None;
            }
        }

        void HandleInMemoryUpdateMessage(MessagingService ms, Contribution contribution)
        {
            if (contribution.ContributionId != Contribution.ContributionId)
                return;

            Contribution = contribution;
        }

        async Task OpenUrl()
            => await Browser.OpenAsync(Contribution.ReferenceUrl, new BrowserLaunchOptions { Flags = BrowserLaunchFlags.PresentAsPageSheet }).ConfigureAwait(false);
    }
}
