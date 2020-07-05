//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using AsyncAwaitBestPractices;
//using AsyncAwaitBestPractices.MVVM;
//using MVP.Models;
//using MVP.Services;
//using Xamarin.Essentials;
//using Xamarin.Forms;

//namespace MVP.PageModels
//{
//    public class WizardAdditionalTechnologyPageModel : BasePageModel
//    {
//        IList<ContributionTechnology> _selectedContributionTechnologies;
//        Contribution _contribution;

//        public IAsyncCommand BackCommand { get; set; }
//        public IAsyncCommand NextCommand { get; set; }
//        public ICommand SelectionChangedCommand { get; set; }

//        public IList<ContributionTechnology> SelectedContributionTechnologies
//        {
//            get { return _selectedContributionTechnologies; }
//            set { _selectedContributionTechnologies = value; }
//        }

//        public IList<MvvmHelpers.Grouping<string, ContributionTechnology>> GroupedContributionTechnologies { get; set; } = new List<MvvmHelpers.Grouping<string, ContributionTechnology>>();

//        public WizardAdditionalTechnologyPageModel()
//        {
//            BackCommand = new AsyncCommand(() => Back());
//            NextCommand = new AsyncCommand(() => Next());
//            SelectionChangedCommand = new Command<IList<object>>((list) => SelectionChanged(list));
//        }

//        public override void Init(object initData)
//        {
//            base.Init(initData);

//            if (initData is Contribution contribution)
//            {
//                _contribution = contribution;
//            }

//            LoadContributionAreas().SafeFireAndForget();
//        }

//        void SelectionChanged(IList<object> obj)
//        {
//            if (obj.Count > 2)
//            {
//                obj.Remove(obj.FirstOrDefault());
//            }

//            _selectedContributionTechnologies = obj.Select(x => x as ContributionTechnology).ToList();
//        }

//        async Task LoadContributionAreas()
//        {
//            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
//            {
//                var categories = await MvpApiService.GetContributionAreasAsync().ConfigureAwait(false);

//                if (categories != null)
//                {
//                    var result = new List<MvvmHelpers.Grouping<string, ContributionTechnology>>();

//                    foreach (var item in categories.SelectMany(x => x.ContributionAreas))
//                    {
//                        result.Add(new MvvmHelpers.Grouping<string, ContributionTechnology>(item.AwardName, item.ContributionTechnology));

//                    }

//                    GroupedContributionTechnologies = result;

//                    // Editing mode
//                    if (_contribution.AdditionalTechnologies != null && _contribution.AdditionalTechnologies.Any())
//                    {
//                        var selectedValues = _contribution.AdditionalTechnologies.Select(x => x.Id).ToList();

//                        _selectedContributionTechnologies = result
//                            .SelectMany(x => x)
//                            .Where(x => selectedValues.Contains(x.Id))
//                            .ToList();

//                        RaisePropertyChanged(nameof(SelectedContributionTechnologies));
//                    }
//                }
//            }
//        }

//        async Task Back()
//        {
//            await CoreMethods.PopPageModel(modal: false, animate: false).ConfigureAwait(false);
//        }

//        async Task Next()
//        {
//            if (_selectedContributionTechnologies != null && _selectedContributionTechnologies.Any())
//            {
//                _contribution.AdditionalTechnologies = new ObservableCollection<ContributionTechnology>(_selectedContributionTechnologies);
//            }

//            await CoreMethods.PushPageModel<WizardDatePageModel>(data: _contribution, modal: false, animate: false).ConfigureAwait(false);
//        }
//    }
//}
