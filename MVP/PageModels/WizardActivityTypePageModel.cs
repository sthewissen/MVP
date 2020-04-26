using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using MVP.Models;
using MVP.Services;
using Xamarin.Essentials;

namespace MVP.PageModels
{
    public class WizardActivityTypePageModel : BasePageModel
    {
        readonly MvpApiService _mvpApiService;
        ContributionType _selectedContribution;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand<Contribution> NextCommand { get; set; }

        public List<ContributionType> ContributionTypes { get; set; } = new List<ContributionType>();

        public ContributionType SelectedContributionType
        {
            get => _selectedContribution;
            set
            {
                _selectedContribution = value;

                if (value != null)
                {
                    // This is where we init a new contribution that we will pass through
                    // this wizard like thing all the way to getting it saved hopefully!
                    var contribution = new Contribution
                    {
                        ContributionType = value
                    };

                    NextCommand.Execute(contribution);
                }
            }
        }

        public WizardActivityTypePageModel(MvpApiService mvpApiService)
        {
            _mvpApiService = mvpApiService;
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand<Contribution>((contribution) => Next(contribution));
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            LoadContributionTypes().SafeFireAndForget();
        }

        async Task LoadContributionTypes()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var types = await _mvpApiService.GetContributionTypesAsync().ConfigureAwait(false);

                if (types != null)
                {
                    ContributionTypes = types.OrderBy(x => x.Name).ToList();
                }
            }
        }

        async Task Back()
        {
            // Pop the entire modal stack instead of just going back one screen.
            await CoreMethods.PopModalNavigationService(animate: true).ConfigureAwait(false);
        }

        async Task Next(Contribution contribution)
        {
            await CoreMethods.PushPageModel<WizardTechnologyPageModel>(data: contribution, modal: false, animate: false).ConfigureAwait(false);
        }
    }
}
