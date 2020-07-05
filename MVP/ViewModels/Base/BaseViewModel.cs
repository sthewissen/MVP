using MVP.Services.Interfaces;
using TinyMvvm;
using TinyNavigationHelper;

namespace MVP.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase
    {
        protected IMvpApiService MvpApiService => (MVP.App.MvpApiService);
        protected IAnalyticsService AnalyticsService { get; }
        protected IAuthService AuthService { get; }
        protected IDialogService DialogService { get; }
        protected INavigationHelper NavigationHelper { get; }

        public BaseViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
        {
            AnalyticsService = analyticsService;
            AuthService = authService;
            DialogService = dialogService;
            NavigationHelper = navigationHelper;
        }
    }
}
