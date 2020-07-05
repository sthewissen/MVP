//using System;
//using System.Threading.Tasks;
//using AsyncAwaitBestPractices.MVVM;
//using MVP.Models;
//using MVP.Services;

//namespace MVP.PageModels
//{
//    public class WizardTitlePageModel : BasePageModel
//    {
//        Contribution _contribution;
//        string _title;

//        public IAsyncCommand BackCommand { get; set; }
//        public IAsyncCommand NextCommand { get; set; }

//        public string Title
//        {
//            get => _title;
//            set
//            {
//                _title = value;

//                if (value != null)
//                {
//                    _contribution.Title = value;
//                }
//            }
//        }

//        public WizardTitlePageModel()
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
//                Title = _contribution.Title;
//            }
//        }

//        async Task Back()
//        {
//            await CoreMethods.PopPageModel(modal: false, animate: false).ConfigureAwait(false);
//        }

//        async Task Next()
//        {
//            await CoreMethods.PushPageModel<WizardUrlPageModel>(data: _contribution, modal: false, animate: false).ConfigureAwait(false);
//        }
//    }
//}