using System;
using MVP.Services.Interfaces;
using TinyMvvm;

namespace MVP.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
        }
    }
}
