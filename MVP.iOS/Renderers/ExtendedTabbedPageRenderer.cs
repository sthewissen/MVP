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

            if (Element is TabbedPage)
            {
                for (var i = 0; i < TabBar.Items.Length; i++)
                {
                    SetTitleAttributes(TabBar.Items[i]);
                }
            }

            base.ViewWillAppear(animated);
        }

        public override void ViewDidLayoutSubviews()
        {
            // Set the fonts here, again, because for some reason on language switch they get forgotten.
            base.ViewDidLayoutSubviews();

            foreach (var item in TabBar.Items)
            {
                SetTitleAttributes(item);
            }
        }

        void SetTitleAttributes(UITabBarItem item)
        {
            if (item == null)
                return;

            // Set the font for the title.
            item.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName((string)Xamarin.Forms.Application.Current.Resources["font_regular"], 13)
            }, UIControlState.Normal);

            item.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName((string)Xamarin.Forms.Application.Current.Resources["font_regular"], 13)
            }, UIControlState.Selected);
        }
    }
}
