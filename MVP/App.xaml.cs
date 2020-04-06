using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MVP.Services;
using Xamarin.Essentials;
using System.Threading.Tasks;
using MVP.PageModels;
using FreshMvvm;
using MVP.Services.Helpers;

namespace MVP
{
    public partial class App : Application
    {
        public static MvpApiService MvpApiService { get; set; }

        public App()
        {
            InitializeComponent();

            // Device.SetFlags(new[] { });

            if (VersionTracking.IsFirstLaunchEver)
            {
                // Upon first launch, show the intro!
                MainPage = FreshPageModelResolver.ResolvePageModel<IntroPageModel>();
            }
            else
            {
                // By default show the main screen, we will update accordingly.
                SwitchToRootNavigation();
            }
        }

        public void SwitchToRootNavigation()
        {
            var nav = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<ContributionsPageModel>(), "MainNavigation");
            MainPage = nav;
        }

        public async Task InitializeMvpService()
        {
            // Grab the auth token and set it to the MVP service.
            var token = await SecureStorage.GetAsync("AccessToken").ConfigureAwait(false);
            var service = new MvpApiService(token);

            if (MvpApiService != null)
            {
                MvpApiService.AccessTokenExpired -= MvpApiService_AccessTokenExpired;
                MvpApiService.RequestErrorOccurred -= MvpApiService_RequestErrorOccurred;
            }

            service.AccessTokenExpired += MvpApiService_AccessTokenExpired;
            service.RequestErrorOccurred += MvpApiService_RequestErrorOccurred;

            MvpApiService = service;
        }

        private static void MvpApiService_RequestErrorOccurred(object sender, ApiServiceEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private static void MvpApiService_AccessTokenExpired(object sender, ApiServiceEventArgs e)
        {
            // throw new NotImplementedException();
        }
    }
}