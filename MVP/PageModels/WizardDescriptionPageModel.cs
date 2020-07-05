//using System;
//using System.Threading.Tasks;
//using AsyncAwaitBestPractices.MVVM;
//using MVP.Models;

//namespace MVP.PageModels
//{
//    public class WizardDescriptionPageModel : BasePageModel
//    {
//        Contribution _contribution;
//        string _description;

//        public IAsyncCommand BackCommand { get; set; }
//        public IAsyncCommand NextCommand { get; set; }

//        public string Description
//        {
//            get => _description;
//            set
//            {
//                _description = value;

//                if (value != null)
//                {
//                    _contribution.Description = value;
//                }
//            }
//        }

//        public WizardDescriptionPageModel()
//        {
//            BackCommand = new AsyncCommand(() => Back());
//            NextCommand = new AsyncCommand(() => Next());
//        }

//        public override void Init(object initData)
//        {
//            base.Init(initData);

//            if (initData is Contribution contribution)
//            {
//                _contribution = contribution;
//                Description = _contribution.Description;
//            }
//        }

//        async Task Back()
//        {
//            await CoreMethods.PopPageModel(modal: false, animate: false).ConfigureAwait(false);
//        }

//        async Task Next()
//        {
//            await CoreMethods.PushPageModel<WizardVisibilityPageModel>(data: _contribution, modal: false, animate: false).ConfigureAwait(false);
//        }
//    }
//}
