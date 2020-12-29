using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
using TinyMvvm;
using TinyMvvm.IoC;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase
    {
        bool isNavigating = false;

        protected App CurrentApp => (App)Xamarin.Forms.Application.Current;

        protected IMvpApiService MvpApiService => App.MvpApiService;
        protected IAnalyticsService AnalyticsService { get; }
        protected IAuthService AuthService => App.AuthService;
        protected INavigationHelper NavigationHelper => App.NavigationHelper;

        public virtual IAsyncCommand BackCommand { get; set; }
        public virtual ICommand PrimaryCommand { get; set; }
        public virtual ICommand SecondaryCommand { get; set; }

        public LayoutState State { get; set; }
        public string CustomStateKey { get; set; }

        public BaseViewModel(IAnalyticsService analyticsService)
        {
            AnalyticsService = analyticsService;
            BackCommand = new AsyncCommand(() => BackAsync());
        }

        protected async Task<bool> VerifyInternetConnection()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Connection to internet is not available
                await DialogService.AlertAsync(Translations.error_offline, Translations.error_offline_title, Translations.ok).ConfigureAwait(false); return false;
            }

            return true;
        }

        protected Task NavigateAsync(string page, object param = null)
        {
            if (isNavigating)
                return Task.CompletedTask;

            isNavigating = true;

            return NavigationHelper.NavigateToAsync(page, param).ContinueWith((x) => isNavigating = false);
        }

        protected Task OpenModalAsync(string page, object data, bool withNavigation)
        {
            if (isNavigating)
                return Task.CompletedTask;

            isNavigating = true;

            return NavigationHelper.OpenModalAsync(page, data, withNavigation).ContinueWith((x) => isNavigating = false);
        }

        protected Task CloseModalAsync()
        {
            if (isNavigating)
                return Task.CompletedTask;

            isNavigating = true;

            return NavigationHelper.CloseModalAsync().ContinueWith((x) => isNavigating = false);
        }

        public virtual Task BackAsync()
        {
            if (isNavigating)
                return Task.CompletedTask;

            isNavigating = true;

            return NavigationHelper.BackAsync().ContinueWith((x) => isNavigating = false);
        }
    }
}
