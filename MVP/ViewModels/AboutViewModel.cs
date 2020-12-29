using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP.Models;
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

        public IList Acknowledgements { get; set; } = new List<Acknowledgement>();

        public AboutViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
            OpenSponsorCommand = new AsyncCommand(OpenSponsors);
            OpenLicensesCommand = new AsyncCommand(OpenLicenses);

            Acknowledgements = new List<Acknowledgement>
            {
                new Acknowledgement { Name = "Mark Allibone", Type = AcknowledgementTypes.Translator },
                new Acknowledgement { Name = "Jorge Castro", Type = AcknowledgementTypes.Translator },
                new Acknowledgement { Name = "Rodrigo Juarez", Type = AcknowledgementTypes.Translator },
                new Acknowledgement { Name = "Luis Pujols", Type = AcknowledgementTypes.Translator }
            };
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
