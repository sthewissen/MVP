using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Extensions;
using MVP.Models;

namespace MVP.PageModels
{
    public class ContributionDetailsPageModel : BasePageModel
    {
        public Contribution Contribution { get; set; }

        public string AnnualQuantityHeader { get; set; } = "Annual quantity";
        public string SecondAnnualQuantityHeader { get; set; } = "Second annual quantity";
        public string AnnualReachHeader { get; set; } = "Annual reach";

        public IAsyncCommand BackCommand { get; set; }

        public ContributionDetailsPageModel()
        {
            BackCommand = new AsyncCommand(() => Back());
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            if (initData is Contribution contribution)
            {
                Contribution = contribution;

                if (contribution.ContributionType.Id.HasValue)
                {
                    var fieldNames = contribution.ContributionType.Id.Value.GetContributionTypeRequirements();

                    AnnualQuantityHeader = fieldNames.Item1;
                    SecondAnnualQuantityHeader = fieldNames.Item2;
                    AnnualReachHeader = fieldNames.Item3;
                }
            }
        }

        async Task Back()
        {
            await CoreMethods.PopPageModel();
        }
    }
}
