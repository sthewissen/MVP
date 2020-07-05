//using System;
//using System.Threading.Tasks;
//using AsyncAwaitBestPractices.MVVM;
//using MVP.Extensions;
//using MVP.Models;

//namespace MVP.PageModels
//{
//    public class WizardDatePageModel : BasePageModel
//    {
//        Contribution _contribution;
//        DateTime _date;

//        public IAsyncCommand BackCommand { get; set; }
//        public IAsyncCommand NextCommand { get; set; }

//        public DateTime MinDate { get; set; } = DateTime.Now.CurrentAwardPeriodStartDate();

//        public DateTime StartDate
//        {
//            get => _date;
//            set
//            {
//                _date = value;

//                if (value != null)
//                {
//                    _contribution.StartDate = value;
//                }
//            }
//        }

//        public WizardDatePageModel()
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
//                StartDate = _contribution.StartDate.HasValue ? _contribution.StartDate.Value : DateTime.Now;
//            }
//        }

//        async Task Back()
//        {
//            await CoreMethods.PopPageModel(modal: false, animate: false).ConfigureAwait(false);
//        }

//        async Task Next()
//        {
//            await CoreMethods.PushPageModel<WizardTitlePageModel>(data: _contribution, modal: false, animate: false).ConfigureAwait(false);
//        }
//    }
//}
