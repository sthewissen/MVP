using System;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Services.Interfaces;
using TinyNavigationHelper;

namespace MVP.ViewModels
{
    public class ContributionFormViewModel : BaseViewModel
    {
        Contribution contribution = new Contribution();

        public bool IsEditing { get; set; }

        public ContributionFormViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contribution)
            {
                this.contribution = contribution;
                IsEditing = contribution.ContributionId.HasValue && contribution.ContributionId.Value > 0;
            }

            //LoadContributionAreas().SafeFireAndForget();
        }

        public async override Task Back()
        {
            // Pop the entire modal stack instead of just going back one screen.
            // This means it's editing mode and there is no way to go back and change activity type.
            await NavigationHelper.CloseModalAsync().ConfigureAwait(false);
        }
    }
}
