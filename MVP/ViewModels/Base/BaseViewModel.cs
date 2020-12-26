using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
using TinyMvvm;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase
    {
        protected App CurrentApp => (App)Xamarin.Forms.Application.Current;

        protected IMvpApiService MvpApiService => App.MvpApiService;
        protected IAnalyticsService AnalyticsService { get; }
        protected IAuthService AuthService => App.AuthService;
        protected INavigationHelper NavigationHelper { get; }

        public virtual IAsyncCommand BackCommand { get; set; }
        public virtual ICommand PrimaryCommand { get; set; }
        public virtual ICommand SecondaryCommand { get; set; }

        public LayoutState State { get; set; }
        public string CustomStateKey { get; set; }

        public BaseViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
        {
            AnalyticsService = analyticsService;
            NavigationHelper = navigationHelper;
            BackCommand = new AsyncCommand(() => Back());
        }

        protected async Task<bool> VerifyInternetConnection()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Connection to internet is not available
                await DialogService.AlertAsync(
                    Translations.error_offline,
                    Translations.error_offline_title,
                    Translations.alert_ok).ConfigureAwait(false);
                return false;
            }

            return true;
        }

        public async virtual Task Back()
            => await NavigationHelper.BackAsync().ConfigureAwait(false);
    }
}
