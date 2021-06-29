using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Models;
using MVP.Pages;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
#if !DEBUG
using Plugin.StoreReview;
#endif
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class ContributionsViewModel : BaseViewModel
    {
        const int pageSize = 20;

        public int ItemThreshold { get; set; } = 2;

        public ObservableCollection<Contribution> Contributions { get; set; } = new ObservableCollection<Contribution>();

        public IAsyncCommand OpenProfileCommand { get; set; }
        public IAsyncCommand RefreshDataCommand { get; set; }
        public IAsyncCommand LoadMoreCommand { get; set; }
        public IAsyncCommand<Contribution> OpenContributionCommand { get; set; }

        public bool IsRefreshing { get; set; }
        public bool IsLoadingMore { get; set; }

        public ContributionsViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            OpenContributionCommand = new AsyncCommand<Contribution>((Contribution c) => OpenContribution(c));
            SecondaryCommand = new AsyncCommand(() => OpenAddContribution());
            RefreshDataCommand = new AsyncCommand(RefreshContributions);
            LoadMoreCommand = new AsyncCommand(() => LoadMore());

            MessagingService.Current.Subscribe<Contribution>(MessageKeys.InMemoryAdd, HandleContributionAddMessage);
            MessagingService.Current.Subscribe<Contribution>(MessageKeys.InMemoryDelete, HandleContributionDeleteMessage);
            MessagingService.Current.Subscribe<Contribution>(MessageKeys.InMemoryUpdate, HandleContributionUpdateMessage);
        }

        public async override Task Initialize()
        {
            await base.Initialize();
            LoadContributions().SafeFireAndForget();

#if !DEBUG
            if (!Settings.IsUsingDemoAccount)
            {
                var count = Settings.StartupCount;
                count++;

                if (count == 5)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        AnalyticsService.Track("Review Requested");
                        CrossStoreReview.Current.RequestReview(false);
                    });
                }

                Settings.StartupCount = count;
            }
#endif
        }

        Task RefreshContributions()
        {
            IsRefreshing = true;
            return LoadContributions(true);
        }

        /// <summary>
        /// Refreshes the list of contributions.
        /// </summary>
        async Task LoadContributions(bool refresh = false)
        {
            ItemThreshold = 2;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                State = LayoutState.Custom;
                CustomStateKey = StateKeys.Offline;
                return;
            }

            try
            {
                State = LayoutState.Loading;

                var contributionsList = await MvpApiService.GetContributionsAsync(0, pageSize).ConfigureAwait(false);

                if (contributionsList == null)
                {
                    State = LayoutState.Error;
                    return;
                }

                Contributions = new ObservableCollection<Contribution>(contributionsList.Contributions.OrderByDescending(x => x.StartDate).ToList());
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex);
                State = LayoutState.Error;
            }
            finally
            {
                IsRefreshing = false;

                if (State != LayoutState.Error)
                    State = Contributions.Count > 0 ? LayoutState.None : LayoutState.Empty;
            }
        }

        /// <summary>
        /// Handles hard refreshing after saving/deleting.
        /// </summary>
        void HandleRefreshContributionsMessage(MessagingService obj)
            => LoadContributions().SafeFireAndForget();

        /// <summary>
        /// Handles refreshing after adding.
        /// </summary>
        void HandleContributionAddMessage(MessagingService obj, Contribution contribution)
        {
            State = LayoutState.Loading;
            Contributions.Add(contribution);
            Contributions = new ObservableCollection<Contribution>(Contributions.OrderByDescending(x => x.StartDate).ToList());
            State = LayoutState.None;
        }

        /// <summary>
        /// Handles refreshing after updating.
        /// </summary>
        void HandleContributionUpdateMessage(MessagingService obj, Contribution contribution)
        {
            var prev = Contributions.FirstOrDefault(x=>x.ContributionId == contribution.ContributionId);

            if (prev != null)
            {
                State = LayoutState.Loading;
                Contributions.Remove(prev);
                Contributions.Add(contribution);
                Contributions = new ObservableCollection<Contribution>(Contributions.OrderByDescending(x => x.StartDate).ToList());
                State = LayoutState.None;
            }
        }

        /// <summary>
        /// Handles refreshing after adding.
        /// </summary>
        void HandleContributionDeleteMessage(MessagingService obj, Contribution contribution)
        {
            State = LayoutState.Loading;
            Contributions.Remove(contribution);
            State = LayoutState.None;
        }

        /// <summary>
        /// Loads more contributions when scrolled to the bottom.
        /// </summary>
        async Task LoadMore()
        {
            if (IsLoadingMore)
                return;

            if (!await VerifyInternetConnection())
                return;

            try
            {
                IsLoadingMore = true;

                var contributionsList = await MvpApiService.GetContributionsAsync(Contributions.Count, pageSize).ConfigureAwait(false);

                if (contributionsList == null)
                {
                    await DialogService.AlertAsync(Translations.error_couldntloadmorecontributions, Translations.error_title, Translations.ok).ConfigureAwait(false);
                    return;
                }

                foreach (var item in contributionsList.Contributions.OrderByDescending(x => x.StartDate))
                {
                    Contributions.Add(item);
                }

                AnalyticsService.Track("More Contributions Loaded");

                // If we've reached the end, change the threshold.
                if (!contributionsList.Contributions.Any())
                {
                    ItemThreshold = -1;
                    return;
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.Report(ex);
                await DialogService.AlertAsync(Translations.error_couldntloadmorecontributions, Translations.error_title, Translations.ok).ConfigureAwait(false);
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        async Task OpenAddContribution(Contribution prefilledData = null)
            => await OpenModalAsync(nameof(ContributionFormPage), prefilledData, true).ConfigureAwait(false);

        async Task OpenContribution(Contribution contribution)
            => await NavigateAsync(nameof(ContributionDetailsPage), contribution).ConfigureAwait(false);
    }
}
