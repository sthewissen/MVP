using MVP.Styles;
using TinyMvvm.Forms;
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
        readonly Xamarin.Forms.NavigationPage navigationMainPage;
        readonly Xamarin.Forms.NavigationPage navigationProfilePage;

        public TabbedMainPage()
        {
            var viewCreator = (TinyMvvmViewCreator)((FormsNavigationHelper)NavigationHelper.Current).ViewCreator;

            mainPage = viewCreator.Create(typeof(ContributionsPage));
            profilePage = viewCreator.Create(typeof(ProfilePage));

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

            navigationMainPage.Title = Translations.tabs_activities;
            navigationProfilePage.Title = Translations.tabs_settings;

            Children.Add(navigationMainPage);
            Children.Add(navigationProfilePage);
        }
    }
}

