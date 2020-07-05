using MVP.Services.Interfaces;
using TinyMvvm;
using TinyMvvm.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace MVP.Pages
{
    public class BaseContentPage<T> : ViewBase<T> where T : ViewModelBase
    {
        protected IAnalyticsService AnalyticsService { get; }

        public BaseContentPage(in IAnalyticsService analyticsService)
        {
            AnalyticsService = analyticsService;
            On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            AnalyticsService.Track($"{GetType().Name} Appeared");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            AnalyticsService.Track($"{GetType().Name} Disappeared");
        }
    }
}
