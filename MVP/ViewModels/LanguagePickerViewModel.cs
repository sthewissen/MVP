using System;
using System.Collections.Generic;
using System.Linq;
using MVP.Pages;
using MVP.Resources;
using MVP.Services.Interfaces;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;
using TinyNavigationHelper;
using MVP.Services;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using System.Globalization;
using MVP.Extensions;

namespace MVP.ViewModels
{
    public class LanguagePickerViewModel : BaseViewModel
    {
        readonly LanguageService languageService;

        public List<string> supportedLanguages = new List<string> { "en", "nl", "es" };

        public IList<LanguageViewModel> SupportedLanguages { get; set; } = new List<LanguageViewModel>();
        public IAsyncCommand<LanguageViewModel> SetAppLanguageCommand { get; set; }

        public LanguagePickerViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper, LanguageService languageService)
            : base(analyticsService, navigationHelper)
        {
            this.languageService = languageService;

            SetAppLanguageCommand = new AsyncCommand<LanguageViewModel>((x) => SetAppLanguage(x));

            LoadLanguages();
        }

        /// <summary>
        /// Loads the list of valid languages.
        /// </summary>
        void LoadLanguages()
        {
            // Not going to translate the top name, as it's the
            // language's native name, which should be the same across all languages.

            var languages = new List<LanguageViewModel>();
            var text = LocalizationResourceManager.Current.CurrentCulture.TextInfo;

            foreach (var item in supportedLanguages)
            {
                languages.Add(new LanguageViewModel
                {
                    Description = text.ToTitleCase(item.GetNativeName()),
                    CurrentLanguageDescription = text.ToTitleCase(IsoNames.LanguageNames.GetName(LocalizationResourceManager.Current.CurrentCulture, item)),
                    CI = item
                });
            }

            SupportedLanguages = languages;

            // Set current selection
            var selected = SupportedLanguages.FirstOrDefault(pro => pro.CI == LocalizationResourceManager.Current.CurrentCulture.TwoLetterISOLanguageName);

            if (selected != null)
                selected.IsSelected = true;
        }

        /// <summary>
        /// Sets the app's language.
        /// </summary>
        async Task SetAppLanguage(LanguageViewModel language)
        {
            try
            {
                foreach (var item in SupportedLanguages)
                    item.IsSelected = false;

                languageService.SetLanguage(language.CI);

                LoadLanguages();

                // Also force the tabs to change
                (CurrentApp.MainPage as TabbedMainPage)?.SetTitles();

                AnalyticsService.Track("Preferred Language Changed", nameof(language.CI), language.CI ?? "null");

                HapticFeedback.Perform(HapticFeedbackType.Click);
            }
            catch (Exception ex)
            {
                await DialogService.AlertAsync(Translations.error_couldntchangelanguage, Translations.error_title, Translations.ok).ConfigureAwait(false);

                AnalyticsService.Report(ex);
            }
        }
    }
}
