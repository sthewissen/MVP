using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Services;
using MVP.Services.Interfaces;
using Newtonsoft.Json;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class LicensesViewModel : BaseViewModel
    {
        public IAsyncCommand<OpenSourceSoftware> OpenLicenseCommand { get; set; }

        public IList<OpenSourceSoftware> Licenses { get; set; } = new List<OpenSourceSoftware>();

        public LicensesViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            OpenLicenseCommand = new AsyncCommand<OpenSourceSoftware>(OpenLicense);
        }

        public override Task Initialize()
        {
            base.Initialize();

            var items = JsonConvert.DeserializeObject<List<OpenSourceSoftware>>(LocalResourceService.GetFile("oss"));
            Licenses = items.OrderBy(x => x.Title).ToList();

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Opens the URL of a given license file.
        /// </summary>
        async Task OpenLicense(OpenSourceSoftware license)
        {
            if (string.IsNullOrEmpty(license?.LicenseUrl))
                return;

            await Browser.OpenAsync(license.LicenseUrl, new BrowserLaunchOptions { Flags = BrowserLaunchFlags.PresentAsPageSheet }).ConfigureAwait(false);

            AnalyticsService.Track("License URL Visited", nameof(license), license.Title);
        }
    }
}
