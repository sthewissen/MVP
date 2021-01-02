using System;
using Android.OS;
using MVP.Helpers;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(MVP.Droid.DependencyServices.Statusbar))]
namespace MVP.Droid.DependencyServices
{
    public class Statusbar : IStatusBar
    {
        public Statusbar()
        {
        }

        public void SetStatusBarColor(OSAppTheme theme)
        {
            // The SetStatusBarcolor is new since API 21
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var color = theme == OSAppTheme.Dark ? (Color)Xamarin.Forms.Application.Current.Resources["black"] : (Color)Xamarin.Forms.Application.Current.Resources["white"];
                var darkStatusBarTint = theme != OSAppTheme.Dark;

                var androidColor = color.ToAndroid();
                var window = CrossCurrentActivity.Current.Activity.Window;
                window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
                window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
                window.SetStatusBarColor(androidColor);

                var flag = (Android.Views.StatusBarVisibility)Android.Views.SystemUiFlags.LightStatusBar;
                window.DecorView.SystemUiVisibility = darkStatusBarTint ? flag : 0;
            }
        }
    }
}
