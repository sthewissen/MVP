using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AsyncAwaitBestPractices.MVVM;
using MVP.Extensions;
using MVP.Helpers;
using MVP.Models;
using MVP.Services;
using MVP.Services.Interfaces;
using MvvmHelpers;
using TinyNavigationHelper;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class ContributionsViewModel : BaseViewModel
    {
        readonly List<Contribution> contributions = new List<Contribution>();
        readonly int pageSize = 10;

        bool isLoadingMore;
        Contribution selectedContribution;

        public IList<Grouping<int, Contribution>> GroupedContributions { get; set; } = new List<Grouping<int, Contribution>>();
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
            OpenProfileCommand = new AsyncCommand(() => OpenProfile());
            OpenContributionCommand = new AsyncCommand<Contribution>((Contribution c) => OpenContribution(c));
            OpenAddContributionCommand = new AsyncCommand(() => OpenAddContribution());
            RefreshDataCommand = new AsyncCommand(() => RefreshContributions());
            LoadMoreCommand = new AsyncCommand(() => LoadMoreContributions());

            ((App)Xamarin.Forms.Application.Current).Resumed += App_Resumed;
        }

        async void App_Resumed(object sender, System.EventArgs e)
        {
            await MainThread.InvokeOnMainThreadAsync(CheckForClipboardUrl);
        }

        //public override void Init(object initData)
        //{
        //    base.Init(initData);

        //    RefreshData().SafeFireAndForget();
        //}

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
            GroupedContributions.Clear();
            contributions.Clear();

            var contributionsList = await MvpApiService.GetContributionsAsync(0, pageSize).ConfigureAwait(false);

            if (contributionsList == null)
                return;

            contributions.AddRange(contributionsList.Contributions);

            foreach (var item in contributions)
            {
                if (!string.IsNullOrEmpty(item.ReferenceUrl))
                {
                    var og = await OpenGraph.ParseUrlAsync(item.ReferenceUrl);
                    item.ImageUrl = og.Image.AbsoluteUri;
                }
            }

            GroupedContributions = contributions.ToGroupedContributions();
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
                        GroupedContributions = contributions.ToGroupedContributions();
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

        async Task CheckForClipboardUrl()
        {
            if (!Clipboard.HasText)
                return;

            var text = await Clipboard.GetTextAsync();

            if (!text.StartsWith("http://") && !text.StartsWith("https://"))
                return;

            var shouldCreateActivity = await DialogService.ConfirmAsync(
                "We notice a URL on your clipboard. Do you want us to pre-fill an activity out of that?",
                "That looks cool!",
                "Yes",
                "No"
            );

            if (!shouldCreateActivity)
                return;

            var ogData = await OpenGraph.ParseUrlAsync(text);

            if (ogData == null)
                return;

            DateTime? dateTime = null;

            if (ogData.Metadata.ContainsKey("article:published_time") &&
                DateTime.TryParse(ogData.Metadata["article:published_time"].Value(), out var activityDate))
            {
                dateTime = activityDate;
            }

            await OpenAddContribution(new Contribution
            {
                Title = HttpUtility.HtmlDecode(ogData.Title),
                ReferenceUrl = ogData.Url.AbsoluteUri,
                Description = ogData.Metadata.ContainsKey("og:description") ? HttpUtility.HtmlDecode(ogData.Metadata["og:description"].Value()) : string.Empty,
                StartDate = dateTime
            });
        }

        async Task OpenProfile()
        {
            //await CoreMethods.PushPageModel<ProfilePageModel>().ConfigureAwait(false);
        }

        async Task OpenAddContribution(Contribution prefilledData = null)
        {
            //var page = FreshPageModelResolver.ResolvePageModel<WizardActivityTypePageModel>(prefilledData);

            //var basicNavContainer = new FreshNavigationContainer(page, nameof(WizardActivityTypePageModel));
            //await CoreMethods.PushNewNavigationServiceModal(basicNavContainer, page.GetModel(), true).ConfigureAwait(false);
        }

        async Task OpenContribution(Contribution contribution)
        {
            //await CoreMethods.PushPageModel<ContributionDetailsPageModel>(data: contribution).ConfigureAwait(false);
        }
    }
}
