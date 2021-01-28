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
        public List<OnboardingItem> OnboardingItems { get; }

        public IntroViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            SecondaryCommand = new AsyncCommand(() => SignIn());
            SignInAsDemoCommand = new AsyncCommand(() => SignInAsDemo());

            OnboardingItems = new List<OnboardingItem> {
                new OnboardingItem {
                    ImageName="onboarding1",
                    Title = Translations.onboarding_1_title,
                    Description = Translations.onboarding_1_description
                },
                // Not sure yet if this is going to make it in.
                new OnboardingItem {
                    ImageName="onboarding2",
                    Title = "Detailed statistics",
                    Description = "Gather additional insights through your contribution statistics. When were you most active? What type of contributions are you favorite?"
                },
                new OnboardingItem {
                    ImageName="onboarding3",
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
                Settings.IsUsingDemoAccount = false;

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
