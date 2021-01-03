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
            var text = LocalizationResourceManager.Current.CurrentCulture.TextInfo;

            foreach (var item in LanguageService.SupportedLanguages)
            {
                languages.Add(new LanguageViewModel
                {
                    Description = text.ToTitleCase(item.GetNativeName()),
                    CurrentLanguageDescription = text.ToTitleCase(IsoNames.LanguageNames.GetName(LocalizationResourceManager.Current.CurrentCulture, item)),
                    CI = item
                });
            }

            SupportedLanguages = languages.OrderBy(x => x.Description).ToList();

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

                languageService.PreferredLanguage = language.CI;
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
            (CurrentApp.MainPage as TabbedMainPage)?.SetTitles();
            HapticFeedback.Perform(HapticFeedbackType.Click);
            MessagingService.Current.SendMessage(MessageKeys.RefreshNeeded);
        }
    }
}