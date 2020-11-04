using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace MVP.Helpers
{
    public enum StatusBarStyle
    {
        // Will behave as normal. 
        // White text on black NavigationBar/in iOS Dark mode and 
        // Black text on white NavigationBar/in iOS Light mode
        Default,
        // Will switch the color of content of StatusBar to black. 
        DarkText,
        // Will switch the color of content of StatusBar to white. 
        WhiteText,
        // Will hide the StatusBar
        Hidden
    }

    public static class StatusBar
    {
        public static readonly BindableProperty StatusBarStyleProperty = BindableProperty.CreateAttached(
            "StatusBarStyle",
            typeof(StatusBarStyle),
            typeof(Page),
            StatusBarStyle.Default);

        public static void SetStatusBarStyle(BindableObject page, StatusBarStyle value)
            => page.SetValue(StatusBarStyleProperty, value);

        public static StatusBarStyle GetStatusBarStyle(BindableObject page)
            => (StatusBarStyle)page.GetValue(StatusBarStyleProperty);

        public static IPlatformElementConfiguration<iOS, Page> SetStatusBarStyle(this IPlatformElementConfiguration<iOS, Page> config, StatusBarStyle value)
        {
            SetStatusBarStyle(config.Element, value);
            return config;
        }

        public static StatusBarStyle GetStatusBarStyle(this IPlatformElementConfiguration<iOS, Page> config)
            => GetStatusBarStyle(config.Element);
    }
}
