using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Models.Enums;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using MvvmHelpers.Commands;
using TinyMvvm;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class AppIconPickerViewModel : BaseViewModel
    {
        public IList<AppIconViewModel> AppIcons { get; set; } = new List<AppIconViewModel> {
            new AppIconViewModel() { Key = (int)AppIcon.Default, ImageUrl = $"icon_{AppIcon.Default.ToString().ToLower()}", Description = Resources.Translations.icon_default },
            new AppIconViewModel() { Key = (int)AppIcon.DarkBlue, ImageUrl = $"icon_{AppIcon.DarkBlue.ToString().ToLower()}", Description = Resources.Translations.icon_darkblue  },
            new AppIconViewModel() { Key = (int)AppIcon.Black, ImageUrl = $"icon_{AppIcon.Black.ToString().ToLower()}", Description = Resources.Translations.icon_black  },
            new AppIconViewModel() { Key = (int)AppIcon.White, ImageUrl = $"icon_{AppIcon.White.ToString().ToLower()}", Description = Resources.Translations.icon_white  }
        };

        public ICommand SetAppIconCommand { get; set; }

        public AppIconPickerViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            SetAppIconCommand = new AsyncCommand<AppIconViewModel>((x) => SetAppIcon(x));
            AppIcons.FirstOrDefault(x => x.Key == Preferences.Get(Settings.AppIcon, Settings.AppIconDefault)).IsSelected = true;
        }

        async Task SetAppIcon(AppIconViewModel icon)
        {
            var iconSwitcher = DependencyService.Get<IIconService>();

            if (iconSwitcher == null)
                return;

            // Null switches to default.
            if ((AppIcon)icon.Key == AppIcon.Default)
                await iconSwitcher?.SwitchAppIcon(null);
            else
                await iconSwitcher?.SwitchAppIcon(((AppIcon)icon.Key).ToString());

            Preferences.Set(Settings.AppIcon, icon.Key);

            foreach (var item in AppIcons)
                item.IsSelected = false;

            AppIcons.FirstOrDefault(x => x.Key == Preferences.Get(Settings.AppIcon, Settings.AppIconDefault)).IsSelected = true;
            RaisePropertyChanged(nameof(AppIcons));
            HapticFeedback.Perform(HapticFeedbackType.Click);
        }
    }
}
