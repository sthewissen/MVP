using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class ThemePickerViewModel : BaseViewModel
    {
        public IList<AppThemeViewModel> AppThemes { get; set; } = new List<AppThemeViewModel> {
            new AppThemeViewModel() { Key = (int)OSAppTheme.Unspecified, Description = Resources.Translations.theme_systemdefault },
            new AppThemeViewModel() { Key = (int)OSAppTheme.Light, Description = Resources.Translations.theme_light  },
            new AppThemeViewModel() { Key = (int)OSAppTheme.Dark, Description = Resources.Translations.theme_dark  }
        };

        public IAsyncCommand<AppThemeViewModel> SetAppThemeCommand { get; set; }

        public ThemePickerViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
            SetAppThemeCommand = new AsyncCommand<AppThemeViewModel>((x) => SetAppTheme(x));
            AppThemes.FirstOrDefault(x => x.Key == Preferences.Get(Settings.AppTheme, Settings.AppThemeDefault)).IsSelected = true;
        }

        /// <summary>
        /// Sets the theme for the app.
        /// </summary>
        async Task SetAppTheme(AppThemeViewModel theme)
        {
            try
            {
                Application.Current.UserAppTheme = (OSAppTheme)theme.Key;
                Preferences.Set(Settings.AppTheme, theme.Key);

                foreach (var item in AppThemes)
                    item.IsSelected = false;

                AppThemes.FirstOrDefault(x => x.Key == Preferences.Get(Settings.AppTheme, Settings.AppThemeDefault)).IsSelected = true;
                RaisePropertyChanged(nameof(AppThemes));

                HapticFeedback.Perform(HapticFeedbackType.Click);

                AnalyticsService.Track("App Theme Changed", nameof(theme), ((OSAppTheme)theme.Key).ToString() ?? "null");
            }
            catch (Exception ex)
            {
                await DialogService.AlertAsync(
                    Translations.error_couldntchangetheme,
                    Translations.alert_error_title,
                    Translations.alert_ok
                ).ConfigureAwait(false);

                AnalyticsService.Report(ex);
            }
        }
    }
}
