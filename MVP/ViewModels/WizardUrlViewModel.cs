using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MVP.Helpers;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class WizardUrlViewModel : BaseViewModel
    {
        Contribution contribution;
        string url;

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
        public bool HasValidUrl { get; set; }

        public WizardUrlViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
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
            try
            {
                if (string.IsNullOrEmpty(Url) || (!Url.StartsWith("http://") && !Url.StartsWith("https://")))
                {
                    Title = string.Empty;
                    Description = string.Empty;
                    ImageUrl = string.Empty;
                    HasValidUrl = false;
                    return;
                }

                var openGraphData = await OpenGraph.ParseUrlAsync(Url);

                if (openGraphData.Metadata.ContainsKey("og:title"))
                    Title = HttpUtility.HtmlDecode(openGraphData.Metadata["og:title"].Value());

                if (openGraphData.Metadata.ContainsKey("og:description"))
                    Description = HttpUtility.HtmlDecode(openGraphData.Metadata["og:description"].Value());

                if (openGraphData.Metadata.ContainsKey("og:image"))
                    ImageUrl = openGraphData.Metadata["og:image"].First().Value;

                HasValidUrl = !string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(ImageUrl);
            }
            catch
            {
                // Catch 404s etc, but don't really care much about it further.
                Title = string.Empty;
                Description = string.Empty;
                ImageUrl = string.Empty;
                HasValidUrl = false;
            }
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
