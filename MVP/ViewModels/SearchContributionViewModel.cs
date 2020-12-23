using MVP.Services.Interfaces;
using TinyMvvm;
using TinyNavigationHelper;

namespace MVP.ViewModels
{
    public class SearchContributionViewModel : BaseViewModel
    {
        public SearchContributionViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
        }
    }
}
