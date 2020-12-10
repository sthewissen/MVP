using MVP.Services.Interfaces;
using TinyMvvm;

namespace MVP.ViewModels
{
    public class BadgesViewModel : BaseViewModel
    {
        public BadgesViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
        }
    }
}
