using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TinyMvvm;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

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

        public ContributionsViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
            OpenContributionCommand = new AsyncCommand<Contribution>((Contribution c) => OpenContribution(c));
            SecondaryCommand = new AsyncCommand(() => OpenAddContribution());
            RefreshDataCommand = new AsyncCommand(() => RefreshContributions(true));
            LoadMoreCommand = new AsyncCommand(() => LoadMore());

            MessagingService.Current.Subscribe(MessageKeys.RefreshNeeded, HandleRefreshContributionsMessage);
        }

        public async override Task Initialize()
        {
            await base.Initialize();
            RefreshContributions().SafeFireAndForget();
        }

        async Task RefreshContributions(bool refresh = false)
        {
            ItemThreshold = 2;

            try
            {
                State = LayoutState.Loading;

                var contributionsList = await MvpApiService.GetContributionsAsync(0, pageSize).ConfigureAwait(false);

                if (contributionsList == null)
                    return;

                Contributions = new ObservableCollection<Contribution>(contributionsList.Contributions.OrderByDescending(x => x.StartDate).ToList());
            }
            finally
            {
                IsRefreshing = false;
                State = Contributions.Count > 0 ? LayoutState.None : LayoutState.Empty;
            }
        }

        void HandleRefreshContributionsMessage(MessagingService obj)
            => RefreshContributions().SafeFireAndForget();

        async Task LoadMore()
        {
            if (IsLoadingMore)
                return;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Connection to internet is not available
                await DialogService.AlertAsync(
                    Translations.alert_error_offline,
                    Translations.alert_error_offlinetitle,
                    Translations.alert_ok).ConfigureAwait(false);
                return;
            }

            try
            {
                IsLoadingMore = true;

                var contributionsList = await MvpApiService.GetContributionsAsync(Contributions.Count, pageSize).ConfigureAwait(false);

                foreach (var item in contributionsList.Contributions.OrderByDescending(x => x.StartDate))
                {
                    Contributions.Add(item);
                }

                // If we've reached the end, change the threshold.
                if (!contributionsList.Contributions.Any())
                {
                    ItemThreshold = -1;
                    return;
                }
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        async Task OpenAddContribution(Contribution prefilledData = null)
            => await NavigationHelper.OpenModalAsync(nameof(ContributionFormPage), prefilledData, true).ConfigureAwait(false);

        async Task OpenContribution(Contribution contribution)
            => await NavigationHelper.NavigateToAsync(nameof(ContributionDetailsPage), contribution).ConfigureAwait(false);
    }
}
