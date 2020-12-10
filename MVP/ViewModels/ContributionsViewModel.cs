using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MVP.Extensions;
using MVP.Helpers;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyMvvm;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class ContributionsViewModel : BaseViewModel
    {
        readonly List<Contribution> contributions = new List<Contribution>();
        readonly int pageSize = 10;

        public IList<Contribution> Contributions { get; set; } = new List<Contribution>();

        public IAsyncCommand OpenProfileCommand { get; set; }
        public IAsyncCommand RefreshDataCommand { get; set; }
        public IAsyncCommand<Contribution> OpenContributionCommand { get; set; }

        public ContributionsViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            OpenContributionCommand = new AsyncCommand<Contribution>((Contribution c) => OpenContribution(c));
            SecondaryCommand = new AsyncCommand(() => OpenAddContribution());
            RefreshDataCommand = new AsyncCommand(() => RefreshContributions());

            //CurrentApp.Resumed += App_Resumed;
        }

        //async void App_Resumed(object sender, System.EventArgs e)
        //{
        //    await MainThread.InvokeOnMainThreadAsync(CheckForClipboardUrl);
        //}

        public async override Task Initialize()
        {
            await base.Initialize();
            RefreshContributions().SafeFireAndForget();
        }

        async Task RefreshContributions()
        {
            Contributions.Clear();
            contributions.Clear();
            try
            {
                State = LayoutState.Loading;

                await Task.Delay(5000);

                Contributions.Clear();
                contributions.Clear();

                var contributionsList = await MvpApiService.GetContributionsAsync(0, pageSize).ConfigureAwait(false);

                if (contributionsList == null)
                    return;

            var contributionsList = await MvpApiService.GetContributionsAsync(0, pageSize).ConfigureAwait(false);
                Contributions = contributionsList.Contributions;

            if (contributionsList == null)
                return;

            Contributions = contributionsList.Contributions;
            }
            finally
            {
                State = LayoutState.None;
            }
        }

        async Task<bool> CheckForClipboardUrl()
        {
            var text = string.Empty;

            try
            {
                if (!Clipboard.HasText)
                    return false;

                text = await Clipboard.GetTextAsync();

                if (string.IsNullOrEmpty(text) || (!text.StartsWith("http://") && !text.StartsWith("https://")))
                    return false;

                var shouldCreateActivity = await DialogService.ConfirmAsync(
                    Resources.Translations.clipboard_alert_description,
                    Resources.Translations.clipboard_alert_title,
                    Resources.Translations.alert_yes,
                    Resources.Translations.alert_no
                );

                if (!shouldCreateActivity)
                    return false;

                var ogData = await OpenGraph.ParseUrlAsync(text);

                if (ogData == null)
                    return false;

                DateTime? dateTime = null;

                if (ogData.Metadata.ContainsKey("article:published_time") &&
                    DateTime.TryParse(ogData.Metadata["article:published_time"].Value(), out var activityDate))
                {
                    dateTime = activityDate;
                }

                var contrib = new Contribution
                {
                    Title = HttpUtility.HtmlDecode(ogData.Title),
                    ReferenceUrl = ogData.Url.AbsoluteUri,
                    Description = ogData.Metadata.ContainsKey("og:description")
                        ? HttpUtility.HtmlDecode(ogData.Metadata["og:description"].Value())
                        : string.Empty,
                    StartDate = dateTime
                };

                await NavigationHelper.OpenModalAsync(nameof(ContributionFormPage), contrib, true).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex, new Dictionary<string, string> { { "clipboard_value", text } });
                return false;
            }
        }

        async Task OpenAddContribution(Contribution prefilledData = null)
        {
            if (Preferences.Get(Settings.UseClipboardUrls, true))
            {
                if (await CheckForClipboardUrl())
                    return;
            }

            await NavigationHelper.OpenModalAsync(nameof(ContributionFormPage), prefilledData, true).ConfigureAwait(false);
        }

        async Task OpenContribution(Contribution contribution)
            => await NavigationHelper.NavigateToAsync(nameof(ContributionDetailsPage), contribution).ConfigureAwait(false);
    }
}
