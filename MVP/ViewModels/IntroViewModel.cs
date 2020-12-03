using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using TinyMvvm;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class IntroViewModel : BaseViewModel
    {
        public IAsyncCommand SignInCommand { get; set; }
        public List<OnboardingItem> OnboardingItems { get; }

        public IntroViewModel(IAnalyticsService analyticsService, IAuthService authService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            SignInCommand = new AsyncCommand(() => SignIn());

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
                    await DialogService.AlertAsync("Couldn't authenticate you using the provided credentials. Are you sure you are using the account associated to your MVP ID?", Alerts.Error, Alerts.OK);
                }
            }
            catch (Exception e)
            {
                AnalyticsService.Report(e);
                await DialogService.AlertAsync(Alerts.UnexpectedError, Alerts.Error, Alerts.OK);
            }
        }
    }
}
