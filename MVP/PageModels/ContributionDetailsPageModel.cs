//using System;
//using System.Threading.Tasks;
//using AsyncAwaitBestPractices.MVVM;
//using FreshMvvm;
//using MVP.Extensions;
//using MVP.Models;
//using MVP.Services;

//namespace MVP.PageModels
//{
//    public class ContributionDetailsPageModel : BasePageModel
//    {
//        public Contribution Contribution { get; set; }
//        public bool CanBeEdited => Contribution != null && Contribution.StartDate.IsWithinCurrentAwardPeriod();

//        public IAsyncCommand DeleteContributionCommand { get; set; }
//        public IAsyncCommand EditContributionCommand { get; set; }

//        public IAsyncCommand BackCommand { get; set; }
//        public ContributionTypeConfig ContributionTypeConfig { get; set; }

//        public ContributionDetailsPageModel()
//        {
//            BackCommand = new AsyncCommand(() => Back());
//            DeleteContributionCommand = new AsyncCommand(() => DeleteContribution());
//            EditContributionCommand = new AsyncCommand(() => EditContribution(), (x) => CanBeEdited);
//        }

//        public override void Init(object initData)
//        {
//            base.Init(initData);

//            if (initData is Contribution contribution)
//            {
//                Contribution = contribution;

//                RaisePropertyChanged(nameof(CanBeEdited));
//                EditContributionCommand.RaiseCanExecuteChanged();

//                if (contribution.ContributionType.Id.HasValue)
//                {
//                    ContributionTypeConfig = contribution.ContributionType.Id.Value.GetContributionTypeRequirements();
//                }
//            }
//        }

//        async Task EditContribution()
//        {
//            // TODO: Change this check to block people if current time is between April 1 and July 1 and the
//            // contribution is before April 1st.
//            if (Contribution.StartDate.IsWithinCurrentAwardPeriod())
//            {
//                var page = FreshPageModelResolver.ResolvePageModel<WizardTechnologyPageModel>(Contribution);
//                var basicNavContainer = new FreshNavigationContainer(page, nameof(WizardTechnologyPageModel));
//                await CoreMethods.PushNewNavigationServiceModal(basicNavContainer, page.GetModel(), true).ConfigureAwait(false);
//            }
//            else
//            {
//                // TODO: Message
//            }
//        }

//        async Task DeleteContribution()
//        {
//            try
//            {
//                // Shouldn't be getting here anyway, so no need for a message.
//                if (!Contribution.StartDate.IsWithinCurrentAwardPeriod())
//                    return;

//                // Ask for confirmation before deletion.
//                if (await _dialogService.ConfirmAsync("Are you sure you want to delete this contribution? You cannot undo this.", Alerts.HoldOn, Alerts.OK, Alerts.Cancel))
//                {
//                    var isDeleted = await MvpApiService.DeleteContributionAsync(Contribution);

//                    if (isDeleted)
//                    {
//                        // Pass back true to indicate it needs to refresh.
//                        await CoreMethods.PopPageModel(true, false, true);
//                    }
//                    else
//                    {
//                        await _dialogService.AlertAsync("Your contribution could not be deleted. Perhaps it was already deleted, or it took place in the previous award period?", Alerts.Error, Alerts.OK);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                _analyticsService.Report(ex);
//                await _dialogService.AlertAsync(Alerts.UnexpectedError, Alerts.Error, Alerts.OK);
//            }
//        }

//        async Task Back()
//        {
//            await CoreMethods.PopPageModel();
//        }
//    }
//}
