//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AsyncAwaitBestPractices;
//using AsyncAwaitBestPractices.MVVM;
//using MVP.Models;
//using Xamarin.Essentials;

//namespace MVP.PageModels
//{
//    public class WizardActivityTypePageModel : BasePageModel
//    {
//        ContributionType _selectedContribution;
//        Contribution _contribution = new Contribution();

//        public IAsyncCommand BackCommand { get; set; }
//        public IAsyncCommand NextCommand { get; set; }

//        public List<ContributionType> ContributionTypes { get; set; } = new List<ContributionType>();

//        public ContributionType SelectedContributionType
//        {
//            get => _selectedContribution;
//            set
//            {
//                _selectedContribution = value;

//                if (value != null)
//                {
//                    _contribution.ContributionType = value;
//                    NextCommand.Execute(_contribution);
//                }
//            }
//        }

//        public WizardActivityTypePageModel()
//        {
//            BackCommand = new AsyncCommand(() => Back());
//            NextCommand = new AsyncCommand(() => Next());
//        }

//        public override void Init(object initData)
//        {
//            base.Init(initData);

//            // If a new contribution is coming in, the user created one from the URL
//            // they had on the clipboard.
//            if (initData is Contribution contribution)
//            {
//                _contribution = contribution;
//            }

//            LoadContributionTypes().SafeFireAndForget();
//        }

//        async Task LoadContributionTypes()
//        {
//            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
//            {
//                var types = await MvpApiService.GetContributionTypesAsync().ConfigureAwait(false);

//                if (types != null)
//                {
//                    ContributionTypes = types.OrderBy(x => x.Name).ToList();
//                }
//            }
//        }

//        async Task Back()
//        {
//            // Pop the entire modal stack instead of just going back one screen.
//            await CoreMethods.PopModalNavigationService(animate: true).ConfigureAwait(false);
//        }

//        async Task Next()
//        {
//            await CoreMethods.PushPageModel<WizardTechnologyPageModel>(data: _contribution, modal: false, animate: false).ConfigureAwait(false);
//        }
//    }
//}
