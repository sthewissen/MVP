using System;
using MVP.Services.Interfaces;
using TinyMvvm;

namespace MVP.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        public StatisticsViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
        }
    }
}
