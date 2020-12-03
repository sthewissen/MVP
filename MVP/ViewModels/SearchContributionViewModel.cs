using MVP.Services.Interfaces;
using TinyMvvm;

namespace MVP.ViewModels
{
    public class SearchContributionViewModel : BaseViewModel
    {
        public SearchContributionViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
        }
    }
}
