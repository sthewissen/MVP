using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Extensions;
using MVP.Models;

namespace MVP.PageModels
{
    public class WizardAmountsPageModel : BasePageModel
    {
        Contribution _contribution;
        int? _annualQuantity;
        int? _secondAnnualQuantity;
        int? _annualReach;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand NextCommand { get; set; }

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

        public WizardAmountsPageModel()
        {
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand(() => Next());
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

        async Task Next()
        {
            //await CoreMethods.PushPageModel<WizardUrlPageModel>(data: _contribution, modal: false, animate: false).ConfigureAwait(false);
        }
    }
}
