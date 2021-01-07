using System;
using System.Collections.Generic;
using System.Linq;
using MVP.Pages;
using MVP.Resources;
using MVP.Services.Interfaces;
using Xamarin.Essentials;
using TinyNavigationHelper;
using MVP.Services;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using MVP.Extensions;
using System.Globalization;
using MVP.Helpers;
using Xamarin.Forms;
using System.Resources;

namespace MVP.ViewModels
{
    public class LanguagePickerViewModel : BaseViewModel
    {
        readonly LanguageService languageService;

        public IList<LanguageViewModel> SupportedLanguages { get; set; } = new List<LanguageViewModel>();
        public IAsyncCommand<LanguageViewModel> SetAppLanguageCommand { get; set; }

        public LanguagePickerViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper, LanguageService languageService)
            : base(analyticsService)
        {
            this.languageService = languageService;
            LanguageService.PreferredLanguageChanged += LanguageService_PreferredLanguageChanged;
            SetAppLanguageCommand = new AsyncCommand<LanguageViewModel>((x) => SetAppLanguage(x));
        }

        public override async Task Initialize()
        {
            await base.Initialize();
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
            var text = new CultureInfo(languageService.PreferredLanguage).TextInfo;

            foreach (var item in LanguageService.SupportedLanguages)
            {
                // Fallback.
                var name = IsoNames.LanguageNames.GetName(new CultureInfo("en"), new CultureInfo(item).TwoLetterISOLanguageName);

                // This lookup will throw an exception if it doesn't exist (e.g. for Norway)
                try
                {
                    name = IsoNames.LanguageNames.GetName(new CultureInfo(languageService.PreferredLanguage), new CultureInfo(item).TwoLetterISOLanguageName);
                }
                catch
                {
                }

                languages.Add(new LanguageViewModel
                {
                    Description = text.ToTitleCase(item.GetNativeName()),
                    CurrentLanguageDescription = text.ToTitleCase(name),
                    CI = item
                });
            }

            SupportedLanguages = languages.OrderBy(x => x.Description).ToList();

            // Set current selection
            var selected = SupportedLanguages.FirstOrDefault(pro => pro.CI == languageService.PreferredLanguage);

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
                languageService.PreferredLanguage = language.CI;

                if (Device.RuntimePlatform == Device.Android)
                    await DialogService.AlertAsync(Translations.please_reboot_app_language_change, Translations.warning_title, Translations.ok);
            }
            catch (Exception ex)
            {
                await DialogService.AlertAsync(Translations.error_couldntchangelanguage, Translations.error_title, Translations.ok).ConfigureAwait(false);

                AnalyticsService.Report(ex);
            }
        }

        /// <summary>
        /// Handles when the language has been changed.
        /// </summary>
        void LanguageService_PreferredLanguageChanged(object sender, string e)
        {
            LoadLanguages();
            HapticFeedback.Perform(HapticFeedbackType.Click);
        }
    }
}