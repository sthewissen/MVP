using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;

namespace MVP.ViewModels
{
    public class WizardDescriptionViewModel : BaseViewModel
    {
        Contribution contribution;
        string description;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand NextCommand { get; set; }

        public string Description
        {
            get => description;
            set
            {
                description = value;

                if (value != null)
                {
                    contribution.Description = value;
                }
            }
        }

        public WizardDescriptionViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand(() => Next());
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contrib)
            {
                contribution = contrib;
                Description = contribution.Description;
            }
        }

        async Task Back()
        {
            await NavigationHelper.BackAsync().ConfigureAwait(false);
        }

        async Task Next()
        {
            await NavigationHelper.NavigateToAsync(nameof(WizardVisibilityPage), contribution).ConfigureAwait(false);
        }
    }
}
