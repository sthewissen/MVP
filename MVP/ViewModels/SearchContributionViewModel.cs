using MVP.Services.Interfaces;
using TinyMvvm;

namespace MVP.ViewModels
{
    public class SearchContributionViewModel : BaseViewModel
    {
        public SearchContributionViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
        }
    }
}
