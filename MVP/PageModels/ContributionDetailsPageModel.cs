using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Extensions;
using MVP.Models;
using MVP.Services;

namespace MVP.PageModels
{
    public class ContributionDetailsPageModel : BasePageModel
    {
        private readonly MvpApiService _mvpApiService;

        public Contribution Contribution { get; set; }

        public string AnnualQuantityHeader { get; set; } = "Annual quantity";
        public string SecondAnnualQuantityHeader { get; set; } = "Second annual quantity";
        public string AnnualReachHeader { get; set; } = "Annual reach";

        public IAsyncCommand DeleteContributionCommand { get; set; }
        public IAsyncCommand EditContributionCommand { get; set; }

        public IAsyncCommand BackCommand { get; set; }

        public ContributionDetailsPageModel(MvpApiService mvpApiService)
        {
            _mvpApiService = mvpApiService;
            BackCommand = new AsyncCommand(() => Back());
            DeleteContributionCommand = new AsyncCommand(() => DeleteContribution());
            EditContributionCommand = new AsyncCommand(() => EditContribution());
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

        async Task EditContribution()
        {

        }

        async Task DeleteContribution()
        {
            try
            {
                // Shouldn't be getting here anyway, so no need for a message.
                if (!Contribution.StartDate.IsWithinCurrentAwardPeriod())
                    return;

                // Ask for confirmation before deletion.
                if (await _dialogService.ConfirmAsync("Are you sure you want to delete this contribution? You cannot undo this.", Alerts.HoldOn, Alerts.OK, Alerts.Cancel))
                {
                    var isDeleted = await _mvpApiService.DeleteContributionAsync(Contribution);

                    if (isDeleted ?? false)
                    {
                        // Pass back true to indicate it needs to refresh.
                        await CoreMethods.PopPageModel(true, false, true);
                    }
                    else
                    {
                        await _dialogService.AlertAsync("Your contribution could not be deleted. Perhaps it was already deleted, or it took place in the previous award period?", Alerts.Error, Alerts.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.AlertAsync("Something went wrong that we didn't expect. An error has been logged and we will look into it as soon as possible.", Alerts.Error, Alerts.OK);
                _analyticsService.Report(ex);
            }
        }

        async Task Back()
        {
            await CoreMethods.PopPageModel();
        }
    }
}
