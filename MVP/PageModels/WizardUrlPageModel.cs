using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Models;
using MVP.Services;

namespace MVP.PageModels
{
    public class WizardUrlPageModel : BasePageModel
    {
        Contribution _contribution;
        string _url;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand NextCommand { get; set; }

        public string Url
        {
            get => _url;
            set
            {
                _url = value;

                if (value != null)
                {
                    _contribution.ReferenceUrl = value;
                }
            }
        }

        public WizardUrlPageModel()
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
                Url = _contribution.ReferenceUrl;
            }
        }

        async Task Back()
        {
            await CoreMethods.PopPageModel(modal: false, animate: false).ConfigureAwait(false);
        }

        async Task Next()
        {
            await CoreMethods.PushPageModel<WizardDescriptionPageModel>(data: _contribution, modal: false, animate: false).ConfigureAwait(false);
        }
    }
}
