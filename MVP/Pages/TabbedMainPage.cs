using System;
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
            var navigationMainPage = new NavigationPage(mainPage);

            BarBackgroundColor = Color.White;
            //BarTextColor = Color.FromHex("#222222");
            //SelectedTabColor = Color.FromHex("#0178D4");

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            //navigationPage.IconImageSource = "";
            navigationMainPage.Title = "Activities";

            profilePage.Title = "Profile";

            Children.Add(navigationMainPage);
            Children.Add(profilePage);
        }
    }
}

