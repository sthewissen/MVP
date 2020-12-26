using System.Threading.Tasks;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public IAsyncCommand OpenLicensesCommand { get; set; }
        public IAsyncCommand OpenSponsorCommand { get; set; }

        public AboutViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
            OpenSponsorCommand = new AsyncCommand(OpenSponsors);
            OpenLicensesCommand = new AsyncCommand(OpenLicenses);
        }

        async Task OpenSponsors()
        {
            await Browser.OpenAsync(Constants.SponsorUrl, new BrowserLaunchOptions { Flags = BrowserLaunchFlags.PresentAsPageSheet }).ConfigureAwait(false);
            AnalyticsService.Track("Sponser URL Visited");
        }

        async Task OpenLicenses()
            => await NavigationHelper.NavigateToAsync(nameof(LicensesPage)).ConfigureAwait(false);
    }
}
