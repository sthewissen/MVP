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

namespace MVP.ViewModels
{
    public class LanguagePickerViewModel : BaseViewModel
    {
        readonly LanguageService languageService;

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
            SupportedLanguages = new List<LanguageViewModel>()
            {
                { new LanguageViewModel{ Description = "English", CurrentLanguageDescription=Resources.Translations.language_english, CI = "en" } },
                { new LanguageViewModel{ Description = "Nederlands", CurrentLanguageDescription=Resources.Translations.language_dutch, CI = "nl" } },
                { new LanguageViewModel{ Description = "Swedish", CurrentLanguageDescription=Resources.Translations.language_dutch, CI = "sv" } }
            };

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
                (CurrentApp.MainPage as TabbedMainPage).SetTitles();

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
