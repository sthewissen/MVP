using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using MVP.Pages;
using MVP.Resources;
using MVP.Services.Interfaces;
using TinyMvvm.IoC;
using TinyMvvm;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;
using TinyNavigationHelper;
using MVP.Services;

namespace MVP.ViewModels
{
    public class LanguagePickerViewModel : BaseViewModel
    {
        readonly LanguageService languageService;

        public IList<LanguageViewModel> SupportedLanguages { get; set; } = new List<LanguageViewModel>();
        public ICommand SetAppLanguageCommand { get; set; }

        public LanguagePickerViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper, LanguageService languageService)
            : base(analyticsService, navigationHelper)
        {
            this.languageService = languageService;

            SetAppLanguageCommand = new Command<LanguageViewModel>((x) => SetAppLanguage(x));

            LoadLanguages();
        }

        void LoadLanguages()
        {
            // Not going to translate the top name, as it's the
            // language's native name, which should be the same across all languages.
            SupportedLanguages = new List<LanguageViewModel>()
            {
                { new LanguageViewModel{ Description = "English", CurrentLanguageDescription=Resources.Translations.language_english, CI = "en" } },
                { new LanguageViewModel{ Description = "Nederlands", CurrentLanguageDescription=Resources.Translations.language_dutch, CI = "nl" } }
            };

            // Set current selection
            var selected = SupportedLanguages.FirstOrDefault(pro => pro.CI == LocalizationResourceManager.Current.CurrentCulture.TwoLetterISOLanguageName);

            if (selected != null)
                selected.IsSelected = true;
        }

        void SetAppLanguage(LanguageViewModel language)
        {
            foreach (var item in SupportedLanguages)
                item.IsSelected = false;

            languageService.SetLanguage(language.CI);

            // SupportedLanguages.FirstOrDefault(x => x.CI == Preferences.Get(Settings.AppLanguage, Settings.AppLanguageDefault)).IsSelected = true;

            LoadLanguages();

            // Also force the tabs to change
            (CurrentApp.MainPage as TabbedMainPage).SetTitles();
            HapticFeedback.Perform(HapticFeedbackType.Click);
        }
    }
}
