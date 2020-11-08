using MVP.Services.Interfaces;
using TinyNavigationHelper;

namespace MVP.ViewModels
{
    public class BadgesViewModel : BaseViewModel
    {
        public BadgesViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
        }
    }
}
