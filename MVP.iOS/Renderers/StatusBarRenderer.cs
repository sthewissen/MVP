using System;
using MVP.Helpers;
using MVP.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Page), typeof(StatusBarRendrerer))]
namespace MVP.iOS.Renderers
{
    public class StatusBarRendrerer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SetStatusBarStyle(StatusBar.GetStatusBarStyle(Element));
        }

        void SetStatusBarStyle(StatusBarStyle statusBarStyle)
        {
            switch (statusBarStyle)
            {
                case StatusBarStyle.DarkText:
                    UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.DarkContent, true);
                    UIApplication.SharedApplication.SetStatusBarHidden(false, true);
                    break;
                case StatusBarStyle.WhiteText:
                    UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
                    UIApplication.SharedApplication.SetStatusBarHidden(false, true);
                    break;
                case StatusBarStyle.Hidden:
                    UIApplication.SharedApplication.SetStatusBarHidden(true, true);
                    break;
                case StatusBarStyle.Default:
                default:
                    UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, true);
                    UIApplication.SharedApplication.SetStatusBarHidden(false, true);
                    break;
            }

            SetNeedsStatusBarAppearanceUpdate();
        }
    }
}