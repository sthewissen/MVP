using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using MVP.Pages;
using MVP.Resources;
using MVP.Services.Interfaces;
using TinyMvvm.IoC;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class LanguagePickerViewModel : BaseViewModel
    {
        public IList<LanguageViewModel> SupportedLanguages { get; set; } = new List<LanguageViewModel>();
        public ICommand SetAppLanguageCommand { get; set; }

        public LanguagePickerViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            SetAppLanguageCommand = new Command<LanguageViewModel>((x) => SetAppLanguage(x));

            LoadLanguages();
        }

        void LoadLanguages()
        {
            SupportedLanguages = new List<LanguageViewModel>()
            {
                { new LanguageViewModel{ Description = Translations.language_english, CI = "en" } },
                { new LanguageViewModel{ Description = Translations.language_dutch, CI = "nl" } }
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

            Preferences.Set(Settings.AppLanguage, language.CI);
            SupportedLanguages.FirstOrDefault(x => x.CI == Preferences.Get(Settings.AppLanguage, Settings.AppLanguageDefault)).IsSelected = true;

            LocalizationResourceManager.Current.SetCulture(CultureInfo.GetCultureInfo(language.CI));
            LoadLanguages();

            // Also force the tabs to change
            (CurrentApp.MainPage as TabbedMainPage).SetTitles();
        }
    }
}
