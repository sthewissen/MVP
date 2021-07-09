using System;
using System.ComponentModel;
using System.Linq;
using Foundation;
using MVP.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(ExtendedNavigationPageRenderer))]
namespace MVP.iOS.Renderers
{
    /// <summary>
    /// I can't help but feel bad here, but apparently changing a font somewhere is tricky business.
    /// </summary>
    public class ExtendedNavigationPageRenderer : NavigationRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
                UpdateStyle();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            UpdateStyle();
            Element.PropertyChanged += HandlePropertyChanged;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            UpdateStyle();
            Element.PropertyChanged -= HandlePropertyChanged;
        }

        void UpdateStyle()
        {
            var navigationBarAppearance = NavigationBar.StandardAppearance;

            var titleTextAttributes = new UIStringAttributes
            {
                Font = UIFont.FromName((string)Xamarin.Forms.Application.Current.Resources["font_semibold"], 16)                
            };

            var title = new UIStringAttributes
            {
                ForegroundColor = Xamarin.Forms.Application.Current.RequestedTheme == OSAppTheme.Light ?
                    ((Color)Xamarin.Forms.Application.Current.Resources["black"]).ToUIColor() :
                    ((Color)Xamarin.Forms.Application.Current.Resources["white"]).ToUIColor(),
                Font = UIFont.FromName((string)Xamarin.Forms.Application.Current.Resources["font_bold"], 18)
            };

            var titleText = new UITextAttributes
            {
                TextColor = title.ForegroundColor,
                Font = title.Font
            };

            navigationBarAppearance.TitleTextAttributes = title;

            if (titleTextAttributes.Dictionary.Keys.Any())
            {
                var dic = new NSDictionary<NSString, NSObject>(
                    new NSString[] { titleTextAttributes.Dictionary.Keys[0] as NSString }, titleTextAttributes.Dictionary.Values
                );

                var button = new UIBarButtonItemAppearance(UIBarButtonItemStyle.Plain);
                button.Normal.TitleTextAttributes = dic;
                button.Highlighted.TitleTextAttributes = dic;

                navigationBarAppearance.ButtonAppearance = button;
            }

            NavigationBar.StandardAppearance = navigationBarAppearance;
            NavigationBar.CompactAppearance = navigationBarAppearance;
            NavigationBar.ScrollEdgeAppearance = navigationBarAppearance;

            UINavigationBar.Appearance.SetTitleTextAttributes(titleText);
        }

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
            => UpdateStyle();
    }
}
