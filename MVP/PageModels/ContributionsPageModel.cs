using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using FreshMvvm;
using MVP.Extensions;
using MVP.Models;
using MVP.Services;
using MvvmHelpers;
using Xamarin.Essentials;

namespace MVP.PageModels
{
    public class ContributionsPageModel : BasePageModel
    {
        readonly MvpApiService _mvpApiService;
        readonly List<Contribution> _contributions = new List<Contribution>();

        int _pageSize = 10;
        bool _isLoadingMore;
        Contribution _selectedContribution;

        public IList<Grouping<int, Contribution>> GroupedContributions { get; set; } = new List<Grouping<int, Contribution>>();
        public Profile Profile { get; set; }
        public string ProfileImage { get; set; }
        public string Name { get; set; }

        public Contribution SelectedContribution
        {
            get => _selectedContribution;
            set
            {
                _selectedContribution = value;
                if (value != null)
                    OpenContributionCommand.Execute(value);
            }
        }

        public IAsyncCommand OpenProfileCommand { get; set; }
        public IAsyncCommand RefreshDataCommand { get; set; }
        public IAsyncCommand<Contribution> OpenContributionCommand { get; set; }
        public IAsyncCommand OpenAddContributionCommand { get; set; }
        public IAsyncCommand LoadMoreCommand { get; set; }

        public int ItemThreshold { get; set; } = 1;

        public ContributionsPageModel(MvpApiService mvpApiService)
        {
            _mvpApiService = mvpApiService;

            OpenProfileCommand = new AsyncCommand(() => OpenProfile());
            OpenContributionCommand = new AsyncCommand<Contribution>((Contribution c) => OpenContribution(c));
            OpenAddContributionCommand = new AsyncCommand(() => OpenAddContribution());
            RefreshDataCommand = new AsyncCommand(() => RefreshContributions());
            LoadMoreCommand = new AsyncCommand(() => LoadMoreContributions());
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            RefreshData().SafeFireAndForget();
        }

        public async override void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);

            if (returnedData is bool refreshData && refreshData)
            {
                await RefreshContributions().ConfigureAwait(false);
            }
        }

        async Task RefreshData()
        {
            await Task.WhenAll(RefreshContributions(), RefreshProfileData()).ConfigureAwait(false);
        }

        async Task RefreshContributions()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                GroupedContributions.Clear();
                _contributions.Clear();

                var contributionsList = await _mvpApiService.GetContributionsAsync(0, _pageSize).ConfigureAwait(false);

                if (contributionsList != null)
                {
                    _contributions.AddRange(contributionsList.Contributions);
                    GroupedContributions = _contributions.ToGroupedContributions();
                }
            }
        }

        async Task RefreshProfileData()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var profile = await _mvpApiService.GetProfileAsync().ConfigureAwait(false);
                var image = await _mvpApiService.GetProfileImageAsync().ConfigureAwait(false);

                Name = profile.FullName;
                ProfileImage = image;
            }
        }

        async Task LoadMoreContributions()
        {
            // Don't load more when we're already doing that.
            if (_isLoadingMore)
                return;

            _isLoadingMore = true;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var contributionsList = await _mvpApiService.GetContributionsAsync(_contributions.Count, _pageSize).ConfigureAwait(false);

                if (contributionsList != null)
                {
                    if (contributionsList.Contributions.Any())
                    {
                        // Add the contributions and regroup the lot.
                        _contributions.AddRange(contributionsList.Contributions);
                        GroupedContributions = _contributions.ToGroupedContributions();
                    }
                    else if (contributionsList.TotalContributions != contributionsList.PagingIndex)
                    {
                        // Stop loading more, because there's no contributions anymore and we're at the end.
                        ItemThreshold = -1;
                    }
                }
            }

            _isLoadingMore = false;
        }

        async Task OpenProfile()
        {
            await CoreMethods.PushPageModel<ProfilePageModel>().ConfigureAwait(false);
        }

        async Task OpenAddContribution()
        {
            var page = FreshPageModelResolver.ResolvePageModel<WizardActivityTypePageModel>();
            var basicNavContainer = new FreshNavigationContainer(page, nameof(WizardActivityTypePageModel));
            await CoreMethods.PushNewNavigationServiceModal(basicNavContainer, page.GetModel(), true).ConfigureAwait(false);
        }

        async Task OpenContribution(Contribution contribution)
        {
            await CoreMethods.PushPageModel<ContributionDetailsPageModel>(data: contribution).ConfigureAwait(false);
        }
    }
}
