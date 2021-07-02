using System.Collections.Generic;
using MVP.iOS.Renderers;
using MVP.Pages;
using TinyMvvm.Forms;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewBase), typeof(ExtendedContentPageRenderer))]
namespace MVP.iOS.Renderers
{
    public class ExtendedContentPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // Ensures the back gesture swipe still works even though we're hiding the native nav bar.
            if (ViewController.NavigationController == null)
                return;

            ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            ViewController.NavigationController.InteractivePopGestureRecognizer.Delegate = new UIGestureRecognizerDelegate();

            var leftNavList = new List<UIBarButtonItem>();
            var rightNavList = new List<UIBarButtonItem>();
            var page = Element as ContentPage;
            var navigationItem = NavigationController.TopViewController.NavigationItem;

            if (page.ToolbarItems.Count != navigationItem.RightBarButtonItems.Length)
                return;

            for (var i = 0; i < page.ToolbarItems.Count; i++)
            {
                var itemPriority = page.ToolbarItems[i].Priority;

                if (itemPriority < 0)
                {
                    var leftNavItems = navigationItem.RightBarButtonItems[i];
                    leftNavList.Add(leftNavItems);
                }
                else if (itemPriority == 0)
                {
                    var rightNavItems = navigationItem.RightBarButtonItems[i];
                    rightNavList.Add(rightNavItems);
                }
            }

            navigationItem.SetLeftBarButtonItems(leftNavList.ToArray(), false);
            navigationItem.SetRightBarButtonItems(rightNavList.ToArray(), false);
        }
    }
}
