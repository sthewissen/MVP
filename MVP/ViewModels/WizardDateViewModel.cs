using System;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class WizardDateViewModel : BaseViewModel
    {
        Contribution contribution;
        DateTime date;

        public IAsyncCommand NextCommand { get; set; }

        public DateTime MinDate { get; set; } = DateTime.Now.CurrentAwardPeriodStartDate();

        public DateTime StartDate
        {
            get => date;
            set
            {
                date = value;

                if (value != null && contribution != null)
                {
                    contribution.StartDate = value;
                }
            }
        }

        public WizardDateViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            NextCommand = new AsyncCommand(() => Next());
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contrib)
            {
                contribution = contrib;
                StartDate = contribution.StartDate.HasValue ? contribution.StartDate.Value : DateTime.Now;
            }
        }

        async Task Back()
        {
            await NavigationHelper.BackAsync().ConfigureAwait(false);
        }

        async Task Next()
        {
            await NavigationHelper.NavigateToAsync(nameof(WizardTitlePage), contribution).ConfigureAwait(false);
        }
    }
}
