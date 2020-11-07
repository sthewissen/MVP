using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MVP.Helpers;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using MvvmHelpers;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class ContributionsViewModel : BaseViewModel
    {
        readonly List<Contribution> contributions = new List<Contribution>();
        readonly int pageSize = 10;

        bool isLoadingMore;
        Contribution selectedContribution;

        public IList<Contribution> Contributions { get; set; } = new List<Contribution>();
        public Profile Profile { get; set; }
        public string ProfileImage { get; set; }
        public string Name { get; set; }

        public Contribution SelectedContribution
        {
            get => selectedContribution;
            set
            {
                selectedContribution = value;

                if (value != null)
                    OpenContributionCommand.Execute(value);
            }
        }

        public IAsyncCommand OpenProfileCommand { get; set; }
        public IAsyncCommand RefreshDataCommand { get; set; }
        public IAsyncCommand<Contribution> OpenContributionCommand { get; set; }
        public IAsyncCommand OpenAddContributionCommand { get; set; }
        public IAsyncCommand LoadMoreCommand { get; set; }

        public ContributionsViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            //OpenProfileCommand = new AsyncCommand(() => OpenProfile());
            OpenContributionCommand = new AsyncCommand<Contribution>((Contribution c) => OpenContribution(c));
            SecondaryCommand = new AsyncCommand(() => OpenAddContribution());
            RefreshDataCommand = new AsyncCommand(() => RefreshContributions());
            //LoadMoreCommand = new AsyncCommand(() => LoadMoreContributions());

            //CurrentApp.Resumed += App_Resumed;
        }

        //async void App_Resumed(object sender, System.EventArgs e)
        //{
        //    await MainThread.InvokeOnMainThreadAsync(CheckForClipboardUrl);
        //}

        public async override Task Initialize()
        {
            await base.Initialize();
            RefreshData().SafeFireAndForget();
        }

        //public async override void ReverseInit(object returnedData)
        //{
        //    base.ReverseInit(returnedData);

        //    if (returnedData is bool refreshData && refreshData)
        //    {
        //        await RefreshContributions().ConfigureAwait(false);
        //    }
        //}

        async Task RefreshData()
        {
            await Task.WhenAll(
                RefreshContributions(),
                RefreshProfileData(),
                RefreshProfileImage()
            ).ConfigureAwait(false);
        }

        async Task RefreshContributions()
        {
            Contributions.Clear();
            contributions.Clear();

            var contributionsList = await MvpApiService.GetContributionsAsync(0, pageSize).ConfigureAwait(false);

            if (contributionsList == null)
                return;

            Contributions = contributionsList.Contributions;
        }

        async Task RefreshProfileImage()
        {
            var image = await MvpApiService.GetProfileImageAsync().ConfigureAwait(false);

            if (image == null)
                return;

            ProfileImage = image;
        }

        async Task RefreshProfileData()
        {
            var profile = await MvpApiService.GetProfileAsync().ConfigureAwait(false);

            if (profile == null)
                return;

            Name = profile.FullName;
        }

        async Task LoadMoreContributions()
        {
            // Don't load more when we're already doing that.
            if (isLoadingMore)
                return;

            isLoadingMore = true;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var contributionsList = await MvpApiService.GetContributionsAsync(contributions.Count, pageSize).ConfigureAwait(false);

                if (contributionsList != null)
                {
                    if (contributionsList.Contributions.Any())
                    {
                        // Add the contributions and regroup the lot.
                        contributions.AddRange(contributionsList.Contributions);
                        Contributions = contributions;
                    }
                    else if (contributionsList.TotalContributions != contributionsList.PagingIndex)
                    {
                        // Stop loading more, because there's no contributions anymore and we're at the end.
                        //ItemThreshold = -1;
                    }
                }
            }

            isLoadingMore = false;
        }

        async Task<bool> CheckForClipboardUrl()
        {
            try
            {
                if (!Clipboard.HasText)
                    return false;

                var text = await Clipboard.GetTextAsync();

                if (string.IsNullOrEmpty(text) || (!text.StartsWith("http://") && !text.StartsWith("https://")))
                    return false;

                var shouldCreateActivity = await DialogService.ConfirmAsync(
                    "We notice a URL on your clipboard. Do you want us to pre-fill an activity out of that?",
                    "That looks cool!",
                    "Yes",
                    "No"
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

                await NavigationHelper.OpenModalAsync(nameof(WizardActivityTypePage), contrib, true).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        async Task OpenProfile()
        {
            await NavigationHelper.NavigateToAsync(nameof(ProfilePage)).ConfigureAwait(false);
        }

        async Task OpenAddContribution(Contribution prefilledData = null)
        {
            if (!await CheckForClipboardUrl())
            {
                await NavigationHelper.OpenModalAsync(nameof(WizardActivityTypePage), prefilledData, true).ConfigureAwait(false);
            }
        }

        async Task OpenContribution(Contribution contribution)
        {
            await NavigationHelper.NavigateToAsync(nameof(ContributionDetailsPage), contribution).ConfigureAwait(false);
        }
    }
}
