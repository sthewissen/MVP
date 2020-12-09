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

        public ThemePickerViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            SetAppThemeCommand = new Command<AppThemeViewModel>((x) => SetAppTheme(x));

            AppThemes.FirstOrDefault(x => x.Key == Preferences.Get(Settings.AppTheme, Settings.AppThemeDefault)).IsSelected = true;
        }

        void SetAppTheme(AppThemeViewModel theme)
        {
            foreach (var item in AppThemes)
                item.IsSelected = false;

            switch (theme.Key)
            {
                case 0:
                    Application.Current.UserAppTheme = OSAppTheme.Unspecified;
                    Preferences.Set(Settings.AppTheme, (int)OSAppTheme.Unspecified);
                    break;
                case 1:
                    Application.Current.UserAppTheme = OSAppTheme.Light;
                    Preferences.Set(Settings.AppTheme, (int)OSAppTheme.Light);
                    break;
                case 2:
                    Application.Current.UserAppTheme = OSAppTheme.Dark;
                    Preferences.Set(Settings.AppTheme, (int)OSAppTheme.Dark);
                    break;
                default:
                    break;
            }

            AppThemes.FirstOrDefault(x => x.Key == Preferences.Get(Settings.AppTheme, Settings.AppThemeDefault)).IsSelected = true;
            RaisePropertyChanged(nameof(AppThemes));
            HapticFeedback.Perform(HapticFeedbackType.Click);
        }
    }
}
