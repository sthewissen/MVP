using MVP.Services;
using MVP.Services.Interfaces;

namespace MVP.PageModels
{
    public class BasePageModel : FreshMvvm.FreshBasePageModel
    {
        protected IDialogService _dialogService;
        protected IAnalyticsService _analyticsService;

        public IMvpApiService MvpApiService => (MVP.App.MvpApiService);

        public BasePageModel()
        {
            _dialogService = AppContainer.Resolve<IDialogService>();
            _analyticsService = AppContainer.Resolve<IAnalyticsService>();
        }
    }
}
