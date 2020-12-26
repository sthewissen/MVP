using System;
using System.Collections.Specialized;
using MVP.Styles;
using TinyMvvm.Forms;
using TinyMvvm;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using MVP.Resources;
using TinyNavigationHelper.Forms;
using TinyNavigationHelper.Abstraction;

namespace MVP.Pages
{
    public class TabbedMainPage : Xamarin.Forms.TabbedPage
    {
        readonly Xamarin.Forms.Page mainPage;
        readonly Xamarin.Forms.Page profilePage;
        readonly Xamarin.Forms.Page badgesPage;
        readonly Xamarin.Forms.Page statsPage;
        readonly Xamarin.Forms.NavigationPage navigationMainPage;
        readonly Xamarin.Forms.NavigationPage navigationProfilePage;

        public TabbedMainPage()
        {
            var viewCreator = (TinyMvvmViewCreator)((FormsNavigationHelper)NavigationHelper.Current).ViewCreator;

            mainPage = viewCreator.Create(typeof(ContributionsPage));
            profilePage = viewCreator.Create(typeof(ProfilePage));
            badgesPage = viewCreator.Create(typeof(BadgesPage));
            statsPage = viewCreator.Create(typeof(StatisticsPage));

            navigationMainPage = new Xamarin.Forms.NavigationPage(mainPage);
            navigationProfilePage = new Xamarin.Forms.NavigationPage(profilePage);

            var fontIcon = (string)Xamarin.Forms.Application.Current.Resources["font_icon"];

            On<iOS>().SetTranslucencyMode(TranslucencyMode.Opaque);
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            navigationMainPage.IconImageSource = new FontImageSource()
            {
                FontFamily = fontIcon,
                Glyph = Icons.home,
                Size = 20
            };

            navigationProfilePage.IconImageSource = new FontImageSource()
            {
                FontFamily = fontIcon,
                Glyph = Icons.settings,
                Size = 20
            };

            statsPage.IconImageSource = new FontImageSource()
            {
                FontFamily = fontIcon,
                Glyph = Icons.activity,
                Size = 20
            };

            badgesPage.IconImageSource = new FontImageSource()
            {
                FontFamily = fontIcon,
                Glyph = Icons.award,
                Size = 20
            };

            navigationMainPage.Title = Translations.tabs_activities;
            statsPage.Title = Translations.tabs_statistics;
            badgesPage.Title = Translations.tabs_badges;
            navigationProfilePage.Title = Translations.tabs_settings;

            Children.Add(navigationMainPage);
            Children.Add(navigationProfilePage);
        }

        /// <summary>
        /// Sets the titles for the tabs again, this is to support dark mode.
        /// For some reason, if you don't call this, the tabs go back to regular font on iOS.
        /// </summary>
        public void SetTitles()
        {
            navigationMainPage.Title = Translations.tabs_activities;
            statsPage.Title = Translations.tabs_statistics;
            badgesPage.Title = Translations.tabs_badges;
            navigationProfilePage.Title = Translations.tabs_settings;
        }
    }
}

