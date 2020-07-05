using System;
using Autofac;
using FormsToolkit.iOS;
using Foundation;
using Microsoft.Identity.Client;
using MVP.Pages;
using MVP.Services;
using MVP.Services.Interfaces;
using MVP.ViewModels;
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

            // Inject our dependencies
            using var scope = ContainerService.Container.BeginLifetimeScope();
            var analyticsService = scope.Resolve<IAnalyticsService>();
            var apiService = scope.Resolve<IMvpApiService>();
            var authService = scope.Resolve<IAuthService>();
            var dialogService = scope.Resolve<IDialogService>();

            LoadApplication(new App(analyticsService, apiService, authService, dialogService));

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return true;
        }
    }
}
