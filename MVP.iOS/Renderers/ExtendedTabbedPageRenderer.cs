using MVP.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(ExtendedTabbedPageRenderer))]
namespace MVP.iOS.Renderers
{
    public class ExtendedTabbedPageRenderer : TabbedRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            if (TabBar?.Items == null)
                return;

            UITabBar.Appearance.ShadowImage = new UIImage();
            UITabBar.Appearance.BackgroundImage = new UIImage();

            if (Element is TabbedPage tabs)
            {
                for (var i = 0; i < TabBar.Items.Length; i++)
                {
                    UpdateTabBarItem(TabBar.Items[i]);
                }
            }

            base.ViewWillAppear(animated);
        }

        void UpdateTabBarItem(UITabBarItem item)
        {
            if (item == null)
                return;

            // Set the font for the title.

            item.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName((OnPlatform<string>)Xamarin.Forms.Application.Current.Resources["font_regular"], 13),
                TextColor = ((Color)Xamarin.Forms.Application.Current.Resources["black"]).ToUIColor()
            }, UIControlState.Normal);

            item.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName((OnPlatform<string>)Xamarin.Forms.Application.Current.Resources["font_regular"], 13),
                TextColor = ((Color)Xamarin.Forms.Application.Current.Resources["primary"]).ToUIColor()
            }, UIControlState.Selected);
        }
    }
}
