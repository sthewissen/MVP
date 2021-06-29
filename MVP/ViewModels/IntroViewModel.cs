using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Pages;
using MVP.Resources;
using MVP.Services;
using MVP.Services.Interfaces;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class IntroViewModel : BaseViewModel
    {
        public IAsyncCommand SignInAsDemoCommand { get; set; }
        public List<OnboardingItem> OnboardingItems { get; set; }

        public IntroViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            SecondaryCommand = new AsyncCommand(() => SignIn());
            SignInAsDemoCommand = new AsyncCommand(() => SignInAsDemo());

            SetOnboardingItems();
        }

        public async override Task OnAppearing()
        {
            await base.OnAppearing();
            Application.Current.RequestedThemeChanged += HandleThemeChanged;
        }

        public async override Task OnDisappearing()
        {
            await base.OnDisappearing();
            Application.Current.RequestedThemeChanged -= HandleThemeChanged;
        }

        void HandleThemeChanged(object sender, AppThemeChangedEventArgs e) => SetOnboardingItems();

        void SetOnboardingItems()
        {
            OnboardingItems = new List<OnboardingItem> {
                new OnboardingItem {
                    ImageName = Application.Current.RequestedTheme == OSAppTheme.Light ? "resource://onboarding1.svg" : "resource://onboarding1d.svg",
                    Title = Translations.onboarding_1_title,
                    Description = Translations.onboarding_1_description
                },
                new OnboardingItem {
                    ImageName = Application.Current.RequestedTheme == OSAppTheme.Light ? "resource://onboarding2.svg" : "resource://onboarding2d.svg",
                    Title = Translations.onboarding_3_title,
                    Description = Translations.onboarding_3_description
                }
            };
        }

        async Task SignInAsDemo()
        {
            Settings.IsUsingDemoAccount = true;
            (Application.Current as App).SwitchDemoMode(true);
            await SignIn();
            AnalyticsService.Track("Demo Mode Activated");
        }

        async Task SignIn()
        {
            try
            {
                await AuthService.SignOutAsync();

                // Pop a sign in request up for the user.
                if(await AuthService.SignInAsync().ConfigureAwait(false))
                {
                    AnalyticsService.Track("User Logged In");

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        NavigationHelper.SetRootView(nameof(TabbedMainPage), false);
                    });
                }
                else
                {
                    await OpenModalAsync(nameof(LoginPage), null, true);
                }
            }
            catch (Exception e)
            {
                AnalyticsService.Report(e);
                await DialogService.AlertAsync(Resources.Translations.error_unexpected, Resources.Translations.error_title, Resources.Translations.ok);
            }
        }

        async Task SignInMsal()
        {
            try
            {
                await AuthService.SignOutAsync();

                // Pop a sign in request up for the user.
                if (await AuthService.SignInAsync().ConfigureAwait(false))
                {
                    AnalyticsService.Track("User Logged In");

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        NavigationHelper.SetRootView(nameof(TabbedMainPage), false);
                    });
                }
                else
                {
                    AnalyticsService.Track("Invalid MVP Account Used");
                    await DialogService.AlertAsync(Resources.Translations.error_nomvpaccount, Resources.Translations.error_title, Resources.Translations.ok);
                }
            }
            catch (Exception e)
            {
                AnalyticsService.Report(e);

                await DialogService.AlertAsync(Resources.Translations.error_unexpected, Resources.Translations.error_title, Resources.Translations.ok);
            }
        }
    }
}
