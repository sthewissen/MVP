using System;
using FormsToolkit.iOS;
using Foundation;
using Microsoft.Identity.Client;
using MVP.Services.Interfaces;
using UIKit;

namespace MVP.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // Init plugins
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            Toolkit.Init();

            // Inject analytics service
            AppContainer.Build();
            var analyticsService = AppContainer.Resolve<IAnalyticsService>();
            LoadApplication(new App(analyticsService));

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return true;
        }
    }
}
