using System;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Models;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using TinyNavigationHelper;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class ContributionFormViewModel : BaseViewModel
    {
        public ContributionViewModel Contribution { get; set; } = new ContributionViewModel();
        public bool IsEditing { get; set; }
        public ContributionTypeConfig ContributionTypeConfig { get; set; }

        public ContributionFormViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            SecondaryCommand = new Command(() => { if (Contribution.IsValid()) return; });
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contribution)
            {
                Contribution = contribution.ToContributionViewModel();

                IsEditing = contribution.ContributionId.HasValue && contribution.ContributionId.Value > 0;

                if (contribution.ContributionType != null && contribution.ContributionType.Id.HasValue)
                {
                    ContributionTypeConfig = contribution.ContributionType.Id.Value.GetContributionTypeRequirements();
                }
            }

            Contribution.AddValidationRules();
            //LoadContributionAreas().SafeFireAndForget();
        }

        // Pop the entire modal stack instead of just going back one screen.
        // This means it's editing mode and there is no way to go back and change activity type.
        public async override Task Back()
            => await NavigationHelper.CloseModalAsync().ConfigureAwait(false);
    }
}
