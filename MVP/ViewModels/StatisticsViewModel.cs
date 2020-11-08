using System;
using MVP.Services.Interfaces;
using TinyNavigationHelper;

namespace MVP.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        public StatisticsViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
        }
    }
}
