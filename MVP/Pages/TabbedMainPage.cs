using System;
using MVP.Styles;
using TinyMvvm.Forms;
using TinyNavigationHelper.Abstraction;
using TinyNavigationHelper.Forms;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace MVP.Pages
{
    public class TabbedMainPage : Xamarin.Forms.TabbedPage
    {
        public TabbedMainPage()
        {
            var viewCreator = (TinyMvvmViewCreator)((FormsNavigationHelper)NavigationHelper.Current).ViewCreator;

            var mainPage = viewCreator.Create(typeof(ContributionsPage));
            var profilePage = viewCreator.Create(typeof(ProfilePage));
            var badgesPage = viewCreator.Create(typeof(BadgesPage));
            var statsPage = viewCreator.Create(typeof(StatisticsPage));
            var navigationMainPage = new NavigationPage(mainPage);
            var fontIcon = (string)Xamarin.Forms.Application.Current.Resources["font_icon"];

            BarBackgroundColor = Color.White;
            SelectedTabColor = ((Color)Xamarin.Forms.Application.Current.Resources["primary"]);
            BarTextColor = ((Color)Xamarin.Forms.Application.Current.Resources["black"]);
            UnselectedTabColor = ((Color)Xamarin.Forms.Application.Current.Resources["black"]);

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            navigationMainPage.IconImageSource = new FontImageSource()
            {
                FontFamily = fontIcon,
                Glyph = Icons.home,
                Size = 20
            };

            profilePage.IconImageSource = new FontImageSource()
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

            navigationMainPage.Title = "Activities";
            statsPage.Title = "Statistics";
            badgesPage.Title = "Badges";
            profilePage.Title = "Settings";

            Children.Add(navigationMainPage);
            Children.Add(statsPage);
            Children.Add(badgesPage);
            Children.Add(profilePage);
        }
    }
}

