using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using FormsToolkit;
using MVP.Models;
using MVP.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.PageModels
{
    public class IntroPageModel : BasePageModel
    {
        readonly AuthService _authService;

        public IAsyncCommand SignInCommand { get; set; }
        public List<OnboardingItem> OnboardingItems { get; }

        public IntroPageModel(AuthService authService)
        {
            _authService = authService;

            SignInCommand = new AsyncCommand(() => SignIn());

            OnboardingItems = new List<OnboardingItem> {
                new OnboardingItem{ ImageName="onboarding1", Title = "Simple and effective", Description = "Manage all of your community activities from the palm of your hand." },
                new OnboardingItem{ ImageName="onboarding2", Title = "Gather insights", Description = "Gather additional insights through your activity statistics." },
                new OnboardingItem{ ImageName="onboarding3", Title = "Quick Add", Description = "Create activities based on URLs in your clipboard data." }
            };
        }

        async Task SignIn()
        {
            try
            {
                // Pop a sign in request up for the user.
                if (await _authService.SignInAsync().ConfigureAwait(false))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var page = FreshMvvm.FreshPageModelResolver.ResolvePageModel<ContributionsPageModel>();
                        var navigation = new FreshMvvm.FreshNavigationContainer(page);
                        Application.Current.MainPage = navigation;
                    });
                }
                else
                {
                    await _dialogService.AlertAsync("Couldn't authenticate you using the provided credentials. Are you sure you are using the account associated to your MVP ID?", Alerts.Error, Alerts.OK);
                }
            }
            catch (Exception e)
            {
                _analyticsService.Report(e);
                await _dialogService.AlertAsync(Alerts.UnexpectedError, Alerts.Error, Alerts.OK);
            }
        }
    }
}
