using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Extensions;
using MVP.Models;
using MVP.Services;

namespace MVP.PageModels
{
    public class WizardAmountsPageModel : BasePageModel
    {
        readonly IMvpApiService _mvpApiService;
        Contribution _contribution;
        int? _annualQuantity;
        int? _secondAnnualQuantity;
        int? _annualReach;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand SaveCommand { get; set; }

        public int? AnnualQuantity
        {
            get => _annualQuantity;
            set
            {
                _annualQuantity = value;
                _contribution.AnnualQuantity = value;
            }
        }

        public int? SecondAnnualQuantity
        {
            get => _secondAnnualQuantity;
            set
            {
                _secondAnnualQuantity = value;
                _contribution.SecondAnnualQuantity = value;
            }
        }

        public int? AnnualReach
        {
            get => _annualReach;
            set
            {
                _annualReach = value;
                _contribution.AnnualReach = value;
            }
        }

        public ContributionTypeConfig ContributionTypeConfig { get; set; }

        public WizardAmountsPageModel(IMvpApiService mvpApiService)
        {
            _mvpApiService = mvpApiService;

            BackCommand = new AsyncCommand(() => Back());
            SaveCommand = new AsyncCommand(() => Save());
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            if (initData is Contribution contribution)
            {
                _contribution = contribution;

                AnnualQuantity = _contribution.AnnualQuantity;
                SecondAnnualQuantity = _contribution.SecondAnnualQuantity;
                AnnualReach = _contribution.AnnualReach;

                if (contribution.ContributionType.Id.HasValue)
                {
                    ContributionTypeConfig = contribution.ContributionType.Id.Value.GetContributionTypeRequirements();
                }
            }
        }

        async Task Back()
        {
            await CoreMethods.PopPageModel(modal: false, animate: false).ConfigureAwait(false);
        }

        async Task Save()
        {
            _contribution.AnnualQuantity = AnnualQuantity;
            _contribution.AnnualReach = AnnualReach;
            _contribution.SecondAnnualQuantity = SecondAnnualQuantity;

            if (_contribution.ContributionId.HasValue)
            {
                var result = await _mvpApiService.UpdateContributionAsync(_contribution);

                if (result ?? false)
                {
                    await CoreMethods.PopModalNavigationService(true);
                }
                else
                {
                    // TODO: Message
                }
            }
            else
            {
                var result = await _mvpApiService.SubmitContributionAsync(_contribution);

                if (result != null)
                {
                    await CoreMethods.PopModalNavigationService(true);
                }
                else
                {
                    // TODO: Message
                }
            }
        }
    }
}
