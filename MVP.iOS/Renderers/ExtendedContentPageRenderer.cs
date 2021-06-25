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
            ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            ViewController.NavigationController.InteractivePopGestureRecognizer.Delegate = new UIGestureRecognizerDelegate();
        }
    }
}
