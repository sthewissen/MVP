using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Models.Enums;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using Xamarin.CommunityToolkit.ObjectModel;
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
            new AppIconViewModel() { Key = (int)AppIcon.White, ImageUrl = $"icon_{AppIcon.White.ToString().ToLower()}", Description = Resources.Translations.icon_white  },
        };

        public ICommand SetAppIconCommand { get; set; }

        public AppIconPickerViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            SetAppIconCommand = new AsyncCommand<AppIconViewModel>((x) => SetAppIcon(x));
            AppIcons.FirstOrDefault(x => x.Key == Preferences.Get(Settings.AppIcon, Settings.AppIconDefault)).IsSelected = true;
        }

        /// <summary>
        /// Sets the app icon to whatever the user chose.
        /// </summary>
        async Task SetAppIcon(AppIconViewModel icon)
        {
            try
            {
                var iconSwitcher = DependencyService.Get<IIconService>();

                if (iconSwitcher == null)
                    return;

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

                AnalyticsService.Track("App Icon Changed", nameof(icon), ((AppIcon)icon.Key).ToString() ?? "null");
            }
            catch (Exception ex)
            {
                await DialogService.AlertAsync(Translations.error_couldntchangeicon, Translations.error_title, Translations.ok).ConfigureAwait(false);

                AnalyticsService.Report(ex);
            }
        }
    }
}
