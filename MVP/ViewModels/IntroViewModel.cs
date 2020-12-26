using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Pages;
using MVP.Services;
using MVP.Services.Interfaces;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class IntroViewModel : BaseViewModel
    {
        public IAsyncCommand SignInAsDemoCommand { get; set; }
        public List<OnboardingItem> OnboardingItems { get; }

        public IntroViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
            SecondaryCommand = new AsyncCommand(() => SignIn());
            SignInAsDemoCommand = new AsyncCommand(() => SignInAsDemo());

            OnboardingItems = new List<OnboardingItem> {
                new OnboardingItem {
                    ImageName="onboarding1",
                    Title = "Simple and effective",
                    Description = "Manage all of your community activities from the palm of your hand."
                },
                new OnboardingItem {
                    ImageName="onboarding2",
                    Title = "Gather insights",
                    Description = "Gather additional insights through your contribution statistics. When were you most active? What type of contributions are you favorite?"
                },
                new OnboardingItem {
                    ImageName="onboarding3",
                    Title = "Quick Add",
                    Description = "Directly create contributions based on a URL on your clipboard. Pre-filling whatever we can for you allowing you to quickly add new contributions."
                }
            };
        }

        async Task SignInAsDemo()
        {
            Preferences.Set(Settings.IsUsingDemoAccount, true);
            (Application.Current as App).SwitchDemoMode(true);
            await SignIn();
            AnalyticsService.Track("Demo Mode Activated");
        }

        async Task SignIn()
        {
            try
            {
                // Pop a sign in request up for the user.
                if (await AuthService.SignInAsync().ConfigureAwait(false))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        NavigationHelper.SetRootView(nameof(TabbedMainPage), false);
                    });
                }
                else
                {
                    await DialogService.AlertAsync(
                        Resources.Translations.alert_error_nomvpaccount,
                        Resources.Translations.alert_error_title,
                        Resources.Translations.alert_ok
                    );
                }
            }
            catch (Exception e)
            {
                AnalyticsService.Report(e);

                await DialogService.AlertAsync(
                    Resources.Translations.alert_error_unexpected,
                    Resources.Translations.alert_error_title,
                    Resources.Translations.alert_ok
                );
            }
        }
    }
}
