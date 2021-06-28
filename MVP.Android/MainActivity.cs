using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Microsoft.Identity.Client;
using Android.Content;
using MVP.Services;
using Plugin.CurrentActivity;
using Acr.UserDialogs;
using MVP.Services.Interfaces;
using Autofac;

namespace MVP.Droid
{
    [Activity(
        Label = "MVPBuzz",
        Theme = "@style/MainTheme.Launcher",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            // Init plugins
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            UserDialogs.Init(this);

            // Inject analytics service
            using var scope = ContainerService.Container.BeginLifetimeScope();
            var analyticsService = scope.Resolve<IAnalyticsService>();
            var apiService = scope.Resolve<IMvpApiService>();
            var authService = scope.Resolve<IAuthService>();
            var languageService = scope.Resolve<LanguageService>();

            LoadApplication(new App(analyticsService, apiService, authService, languageService));

            // Set the current activity so the AuthService knows where to start.
            MsalAuthService.ParentWindow = CrossCurrentActivity.Current.Activity;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}