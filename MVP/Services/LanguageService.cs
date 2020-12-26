using System.Globalization;
using MVP.Services.Interfaces;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;

namespace MVP.Services
{
    /// <summary>
    /// Changes the app language on the fly.
    /// </summary>
    public class LanguageService
    {
        readonly IAnalyticsService analyticsService;

        public LanguageService(IAnalyticsService analyticsService)
            => this.analyticsService = analyticsService;

        public void SetLanguage(string culture)
        {
            var currentCulture = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? CultureInfo.DefaultThreadCurrentCulture?.Name;

            if (currentCulture != culture && culture != null)
            {
                Preferences.Set(Settings.AppLanguage, culture);
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                CultureInfo.CurrentUICulture = new CultureInfo(culture);
                LocalizationResourceManager.Current.SetCulture(CultureInfo.GetCultureInfo(culture));
            }

            analyticsService.Track("Preferred Language Changed", nameof(culture), culture ?? "null");
        }
    }
}
