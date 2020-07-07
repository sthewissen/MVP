using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AsyncAwaitBestPractices.MVVM;
using MVP.Helpers;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;

namespace MVP.ViewModels
{
    public class WizardUrlViewModel : BaseViewModel
    {
        Contribution contribution;
        string url;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand NextCommand { get; set; }
        public IAsyncCommand GetOpenGraphDataCommand { get; set; }

        public string Url
        {
            get => url;
            set
            {
                url = value;

                if (value != null)
                {
                    // HACK: Remove after
                    if (contribution == null)
                        contribution = new Contribution();

                    contribution.ReferenceUrl = value;

                    GetOpenGraphDataCommand.Execute(value);
                }
            }
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public WizardUrlViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand(() => Next());
            GetOpenGraphDataCommand = new AsyncCommand(() => GetOpenGraphData());
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contrib)
            {
                contribution = contrib;
                Url = contribution.ReferenceUrl;
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
            await NavigationHelper.BackAsync().ConfigureAwait(false);
        }

        async Task Next()
        {
            await NavigationHelper.NavigateToAsync(nameof(WizardDescriptionPage), contribution).ConfigureAwait(false);
        }
    }
}
