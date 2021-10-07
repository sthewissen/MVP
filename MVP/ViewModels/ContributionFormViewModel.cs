using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MVP.Extensions;
using MVP.Helpers;
using MVP.Models;
using MVP.Pages;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class ContributionFormViewModel : BaseViewModel
    {
        public bool IsEditing { get; set; }
        public bool IsContributionValid { get; set; } = true;

        public ContributionViewModel Contribution { get; set; } = new ContributionViewModel();

        public string ContributionDeadlineText
        {
            get
            {
                var startDate = DateTime.Now.CurrentAwardPeriodStartDate();

                return string.Format(
                    Translations.contributionform_timeframememo,
                    startDate.ToLongDateString(),
                    startDate.AddYears(1).AddDays(-1).ToLongDateString()
                );
            }
        }

        public DateTime CurrentAwardPeriodStartDate { get; set; } = DateTime.Now.CurrentAwardPeriodStartDate();

        public IAsyncCommand PickAdditionalTechnologiesCommand { get; set; }
        public IAsyncCommand PickContributionTypeCommand { get; set; }
        public IAsyncCommand PickVisibilityCommand { get; set; }
        public IAsyncCommand PickContributionTechnologyCommand { get; set; }

        public ContributionFormViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            PickAdditionalTechnologiesCommand = new AsyncCommand(() => PickAdditionalTechnologies());
            PickContributionTypeCommand = new AsyncCommand(() => PickContributionType(), (x) => !IsEditing);
            PickVisibilityCommand = new AsyncCommand(() => PickVisibility());
            PickContributionTechnologyCommand = new AsyncCommand(PickContributionTechnology);
            SecondaryCommand = new AsyncCommand(() => SaveContribution());
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contribution)
            {
                Contribution = contribution.ToContributionViewModel();
                IsEditing = contribution.ContributionId.HasValue && contribution.ContributionId.Value > 0;
                PickContributionTypeCommand.RaiseCanExecuteChanged();
            }

            Contribution.AddValidationRules();

            if (!IsEditing)
                await CheckForClipboardUrl();
        }

        /// <summary>
        /// Checks if the clipboard contains a URL to use for prefilling.
        /// </summary>
        async Task CheckForClipboardUrl()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                return;

            if (!Settings.UseClipboardUrls)
                return;

            var clipboardText = string.Empty;

            try
            {
                if (!Clipboard.HasText)
                    return;

                clipboardText = await Clipboard.GetTextAsync();

                var result = Uri.TryCreate(clipboardText, UriKind.Absolute, out var uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (!result)
                    return;

                var shouldCreateActivity = await DialogService.ConfirmAsync(Translations.clipboard_description, Translations.clipboard_title, Translations.yes, Translations.no);

                if (!shouldCreateActivity)
                    return;

                GetOpenGraphData(clipboardText).SafeFireAndForget();
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex, new Dictionary<string, string> { { nameof(clipboardText), clipboardText } });
                return;
            }
        }

        /// <summary>
        /// Gets Open Graph data for prefilling.
        /// </summary>
        async Task GetOpenGraphData(string clipboardText)
        {
            try
            {
                State = LayoutState.Loading;

                var ogData = await OpenGraph.ParseUrlAsync(clipboardText);

                if (ogData == null)
                    return;

                DateTime? dateTime = null;

                if (ogData.Metadata.ContainsKey("article:published_time") &&
                    DateTime.TryParse(ogData.Metadata["article:published_time"].Value(), out var activityDate))
                {
                    dateTime = activityDate.Date;
                }

                Contribution.Title = new Validation.ValidatableObject<string> { Value = HttpUtility.HtmlDecode(ogData.Title) };
                Contribution.ReferenceUrl = new Validation.ValidatableObject<string> { Value = ogData.Url?.AbsoluteUri };
                Contribution.Description = ogData.Metadata.ContainsKey("og:description")
                    ? HttpUtility.HtmlDecode(ogData.Metadata["og:description"].Value())
                    : string.Empty;

                if (dateTime.HasValue)
                    Contribution.StartDate = dateTime.Value.Date;
            }
            catch (Exception ex)
            {
                // Fail silently.
                AnalyticsService.Report(ex, new Dictionary<string, string> { { nameof(clipboardText), clipboardText } });
            }
            finally
            {
                State = LayoutState.None;
            }
        }

        /// <summary>
        /// Saves a contribution.
        /// </summary>
        async Task SaveContribution()
        {
            try
            {
                if (!await VerifyInternetConnection())
                    return;

                if (!Contribution.IsValid())
                {
                    IsContributionValid = false;
                    return;
                }

                IsContributionValid = true;

                State = LayoutState.Saving;

                if (IsEditing)
                {
                    var result = await MvpApiService.UpdateContributionAsync(Contribution.ToContribution());

                    if (!result)
                    {
                        await DialogService.AlertAsync(Translations.error_couldntsavecontribution, Translations.error_title, Translations.ok).ConfigureAwait(false);
                        return;
                    }

                    MainThread.BeginInvokeOnMainThread(() => HapticFeedback.Perform(HapticFeedbackType.LongPress));
                    AnalyticsService.Track("Contribution Edited");
                    await BackAsync().ConfigureAwait(false);

                    MessagingService.Current.SendMessage(MessageKeys.HardRefreshNeeded);
                }
                else
                {
                    var result = await MvpApiService.SubmitContributionAsync(Contribution.ToContribution());

                    if (result == null)
                    {
                        await DialogService.AlertAsync(Translations.error_couldntsavecontribution, Translations.error_title, Translations.ok).ConfigureAwait(false);
                        return;
                    }

                    await SaveSuggestions();

                    AnalyticsService.Track("Contribution Added");
                    MainThread.BeginInvokeOnMainThread(() => HapticFeedback.Perform(HapticFeedbackType.LongPress));
                    await BackAsync().ConfigureAwait(false);

                    MessagingService.Current.SendMessage(MessageKeys.HardRefreshNeeded);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex);

                await DialogService.AlertAsync(Translations.error_couldntsavecontribution, Translations.error_title, Translations.ok).ConfigureAwait(false);
            }
            finally
            {
                State = LayoutState.None;
            }
        }

        async Task SaveSuggestions()
        {
            try
            {
                if (Contribution.ContributionType.Value.Id.HasValue)
                    await SuggestionService.StoreContributionTypeSuggestionAsync(Contribution.ContributionType.Value.Id.Value);

                if (Contribution.ContributionTechnology.Value.Id.HasValue)
                    await SuggestionService.StoreContributionTechnologySuggestionAsync(Contribution.ContributionTechnology.Value.Id.Value);

                if (Contribution.AdditionalTechnologies.Any())
                    await SuggestionService.StoreContributionTechnologySuggestionsAsync(Contribution.AdditionalTechnologies.Select(x => x.Id.Value).ToList());
            }
            catch (Exception ex)
            {
                // Non-essential so not reporting back to the user.
                AnalyticsService.Report(ex);
            }
        }

        // Pop the entire modal stack instead of just going back one screen.
        // This means it's editing mode and there is no way to go back and change activity type.
        public async override Task BackAsync()
        {
            if (Device.RuntimePlatform == Device.iOS)
                await CloseModalAsync().ConfigureAwait(false);
            else
                await base.BackAsync().ConfigureAwait(false);
        }

        async Task PickAdditionalTechnologies()
            => await NavigateAsync(nameof(AdditionalTechnologyPickerPage), Contribution).ConfigureAwait(false);

        async Task PickContributionTechnology()
            => await NavigateAsync(nameof(ContributionTechnologyPickerPage), Contribution).ConfigureAwait(false);

        async Task PickVisibility()
            => await NavigateAsync(nameof(VisibilityPickerPage), Contribution).ConfigureAwait(false);

        async Task PickContributionType()
            => await NavigateAsync(nameof(ContributionTypePickerPage), Contribution).ConfigureAwait(false);
    }
}
