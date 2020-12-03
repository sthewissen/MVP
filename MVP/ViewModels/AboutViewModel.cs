using System;
using MVP.Services.Interfaces;
using TinyMvvm;

namespace MVP.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
        }
    }
}
