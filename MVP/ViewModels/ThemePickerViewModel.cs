using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MVP.Services.Interfaces;
using TinyMvvm;
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

        public ICommand SetAppThemeCommand { get; set; }

        public ThemePickerViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
            SetAppThemeCommand = new Command<AppThemeViewModel>((x) => SetAppTheme(x));
            AppThemes.FirstOrDefault(x => x.Key == Preferences.Get(Settings.AppTheme, Settings.AppThemeDefault)).IsSelected = true;
        }

        void SetAppTheme(AppThemeViewModel theme)
        {
            Application.Current.UserAppTheme = (OSAppTheme)theme.Key;
            Preferences.Set(Settings.AppTheme, theme.Key);

            foreach (var item in AppThemes)
                item.IsSelected = false;

            AppThemes.FirstOrDefault(x => x.Key == Preferences.Get(Settings.AppTheme, Settings.AppThemeDefault)).IsSelected = true;

            RaisePropertyChanged(nameof(AppThemes));
            HapticFeedback.Perform(HapticFeedbackType.Click);
        }
    }
}
