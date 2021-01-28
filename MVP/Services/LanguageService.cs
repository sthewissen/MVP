using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using MVP.Resources;
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
        readonly static WeakEventManager<string?> preferredLanguageChangedEventManager = new();
        readonly IAnalyticsService analyticsService;

        public static List<string> SupportedLanguages = new List<string> { "en", "nl", "es", "de", "tr", "hu", "sv", "no", "it", "fr", "bs" };

        public LanguageService(IAnalyticsService analyticsService)
        {
            this.analyticsService = analyticsService;
        }

        public static event EventHandler<string?> PreferredLanguageChanged
        {
            add => preferredLanguageChangedEventManager.AddEventHandler(value);
            remove => preferredLanguageChangedEventManager.RemoveEventHandler(value);
        }

        public string PreferredLanguage
        {
            get => Settings.AppLanguage;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    value = null;
                else if (!SupportedLanguages.Contains(value))
                    throw new Exception($"Can't set language to {value} as it is not a supported language.");

                Settings.AppLanguage = value;
            }
        }

        public void Initialize() => SetLanguage(PreferredLanguage);

        void SetLanguage(in string ci)
        {
            var currentCulture = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? CultureInfo.DefaultThreadCurrentCulture?.Name;

            if (currentCulture != ci && ci != null)
            {
                var culture = new CultureInfo(ci);

                analyticsService.Track("Preferred Language Set", nameof(ci), ci ?? "null");

                Translations.Culture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                OnPreferredLanguageChanged(ci);
            }
        }

        void OnPreferredLanguageChanged(in string? culture) => preferredLanguageChangedEventManager.RaiseEvent(this, culture, nameof(PreferredLanguageChanged));
    }
}
