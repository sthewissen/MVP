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
    public class WizardTechnologyPageModel : BasePageModel
    {
        ContributionTechnology _selectedContributionTechnology;
        Contribution _contribution;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand<Contribution> NextCommand { get; set; }

        public IList<MvvmHelpers.Grouping<string, ContributionTechnology>> GroupedContributionTechnologies { get; set; } = new List<MvvmHelpers.Grouping<string, ContributionTechnology>>();

        public ContributionTechnology SelectedContributionTechnology
        {
            get => _selectedContributionTechnology;
            set
            {
                _selectedContributionTechnology = value;

                if (value != null)
                {
                    _contribution.ContributionTechnology = value;
                    NextCommand.Execute(_contribution);
                }
            }
        }

        public WizardTechnologyPageModel()
        {
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand<Contribution>((contribution) => Next(contribution));
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            if (initData is Contribution contribution)
            {
                _contribution = contribution;
            }

            LoadContributionAreas().SafeFireAndForget();
        }

        async Task LoadContributionAreas()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var categories = await MvpApiService.GetContributionAreasAsync().ConfigureAwait(false);

                if (categories != null)
                {
                    var result = new List<MvvmHelpers.Grouping<string, ContributionTechnology>>();

                    foreach (var item in categories.SelectMany(x => x.ContributionAreas))
                    {
                        result.Add(new MvvmHelpers.Grouping<string, ContributionTechnology>(item.AwardName, item.ContributionTechnology));
                    }

                    GroupedContributionTechnologies = result;

                    // Editing mode
                    if (_contribution.ContributionTechnology != null)
                    {
                        _selectedContributionTechnology = result
                            .SelectMany(x => x)
                            .FirstOrDefault(x => x.Id == _contribution.ContributionTechnology.Id);

                        RaisePropertyChanged(nameof(SelectedContributionTechnology));
                    }
                }
            }
        }

        async Task Back()
        {
            if (_contribution.ContributionId.HasValue)
            {
                // Pop the entire modal stack instead of just going back one screen.
                // This means it's editing mode and there is no way to go back and change activity type.
                await CoreMethods.PopModalNavigationService(animate: true).ConfigureAwait(false);
            }
            else
            {
                await CoreMethods.PopPageModel(modal: false, animate: false).ConfigureAwait(false);
            }
        }

        async Task Next(Contribution contribution)
        {
            await CoreMethods.PushPageModel<WizardAdditionalTechnologyPageModel>(data: contribution, modal: false, animate: false).ConfigureAwait(false);
        }
    }
}
