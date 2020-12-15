using System;
using System.Threading.Tasks;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyMvvm;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public IAsyncCommand OpenLicensesCommand { get; set; }

        public AboutViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
            OpenLicensesCommand = new AsyncCommand(OpenLicenses);
        }

        async Task OpenLicenses()
            => await NavigationHelper.NavigateToAsync(nameof(LicensesPage)).ConfigureAwait(false);
    }
}
