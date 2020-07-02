using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AsyncAwaitBestPractices.MVVM;
using MVP.Helpers;
using MVP.Models;
using MVP.Services;
using MVP.Services.Interfaces;

namespace MVP.PageModels
{
    public class WizardUrlPageModel : BasePageModel
    {
        Contribution _contribution;
        string _url;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand NextCommand { get; set; }
        public IAsyncCommand GetOpenGraphDataCommand { get; set; }

        public string Url
        {
            get => _url;
            set
            {
                _url = value;

                if (value != null)
                {
                    // HACK: Remove after
                    if (_contribution == null)
                        _contribution = new Contribution();

                    _contribution.ReferenceUrl = value;

                    GetOpenGraphDataCommand.Execute(value);
                }
            }
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public WizardUrlPageModel()
        {
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand(() => Next());
            GetOpenGraphDataCommand = new AsyncCommand(() => GetOpenGraphData());
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

        async Task GetOpenGraphData()
        {
            var openGraphData = await OpenGraph.ParseUrlAsync(Url);

            if (openGraphData.Metadata.ContainsKey("og:title"))
                Title = HttpUtility.HtmlDecode(openGraphData.Metadata["og:title"].Value());

            if (openGraphData.Metadata.ContainsKey("og:description"))
                Description = HttpUtility.HtmlDecode(openGraphData.Metadata["og:description"].Value());

            if (openGraphData.Metadata.ContainsKey("og:image"))
                ImageUrl = openGraphData.Metadata["og:image"].First().Value;
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
