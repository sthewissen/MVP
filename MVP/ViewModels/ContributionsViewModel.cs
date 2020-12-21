using System;
using System.Collections.Generic;
using System.Linq;
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

        public ContributionsViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
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

                Contributions.Clear();
                contributions.Clear();

                var contributionsList = await MvpApiService.GetContributionsAsync(0, pageSize).ConfigureAwait(false);

                if (contributionsList == null)
                    return;

                Contributions = contributionsList.Contributions.OrderByDescending(x => x.StartDate).ToList();
            }
            finally
            {
                State = LayoutState.None;
            }
        }

        async Task OpenAddContribution(Contribution prefilledData = null)
            => await NavigationHelper.OpenModalAsync(nameof(ContributionFormPage), prefilledData, true).ConfigureAwait(false);

        async Task OpenContribution(Contribution contribution)
            => await NavigationHelper.NavigateToAsync(nameof(ContributionDetailsPage), contribution).ConfigureAwait(false);
    }
}
