using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Extensions;
using MVP.Models;
using MVP.Services;
using MVP.Services.Interfaces;
using TinyNavigationHelper;

namespace MVP.ViewModels
{
    public class WizardAmountsViewModel : BaseViewModel
    {
        Contribution contribution;
        int? annualQuantity;
        int? secondAnnualQuantity;
        int? annualReach;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand SaveCommand { get; set; }

        public int? AnnualQuantity
        {
            get => annualQuantity;
            set
            {
                annualQuantity = value;
                contribution.AnnualQuantity = value;
            }
        }

        public int? SecondAnnualQuantity
        {
            get => secondAnnualQuantity;
            set
            {
                secondAnnualQuantity = value;
                contribution.SecondAnnualQuantity = value;
            }
        }

        public int? AnnualReach
        {
            get => annualReach;
            set
            {
                annualReach = value;
                contribution.AnnualReach = value;
            }
        }

        public ContributionTypeConfig ContributionTypeConfig { get; set; }

        public WizardAmountsViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            BackCommand = new AsyncCommand(() => Back());
            SaveCommand = new AsyncCommand(() => Save());
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is Contribution contrib)
            {
                contribution = contrib;

                AnnualQuantity = contribution.AnnualQuantity;
                SecondAnnualQuantity = contribution.SecondAnnualQuantity;
                AnnualReach = contribution.AnnualReach;

                if (contrib.ContributionType.Id.HasValue)
                {
                    ContributionTypeConfig = contrib.ContributionType.Id.Value.GetContributionTypeRequirements();
                }
            }
        }

        async Task Back()
        {
            await NavigationHelper.BackAsync().ConfigureAwait(false);
        }

        async Task Save()
        {
            contribution.AnnualQuantity = AnnualQuantity ?? 0;
            contribution.AnnualReach = AnnualReach ?? 0;
            contribution.SecondAnnualQuantity = SecondAnnualQuantity ?? 0;

            if (contribution.ContributionId.HasValue)
            {
                var result = await MvpApiService.UpdateContributionAsync(contribution);

                if (result)
                {
                    await NavigationHelper.CloseModalAsync();
                }
                else
                {
                    // TODO: Message
                }
            }
            else
            {
                var result = await MvpApiService.SubmitContributionAsync(contribution);

                if (result != null)
                {
                    await NavigationHelper.CloseModalAsync();
                }
                else
                {
                    // TODO: Message
                }
            }
        }
    }
}
