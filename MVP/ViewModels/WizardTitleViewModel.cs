using System.Threading.Tasks;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class WizardTitleViewModel : BaseViewModel
    {
        Contribution contribution;
        string title;

        public IAsyncCommand NextCommand { get; set; }

        public string Title
        {
            get => title;
            set
            {
                title = value;

                if (value != null)
                {
                    contribution.Title = value;
                }
            }
        }

        public WizardTitleViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
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
                Title = contribution.Title;
            }
        }

        async Task Next()
        {
            await NavigationHelper.NavigateToAsync(nameof(WizardUrlPage), contribution).ConfigureAwait(false);
        }
    }
}