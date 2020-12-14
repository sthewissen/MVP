using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using Newtonsoft.Json;
using TinyMvvm;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace MVP.ViewModels
{
    public class SplashScreenViewModel : BaseViewModel
    {
        string fetchText = " ";

        public IAsyncCommand PrefetchDataCommand { get; }

        public string FetchText
        {
            get => fetchText;
            set => Set(ref fetchText, value);
        }

        public SplashScreenViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
            PrefetchDataCommand = new AsyncCommand(() => PrefetchData());
        }

        async Task PrefetchData()
        {
            try
            {

                //var random = new Random();
                //var cont = new List<Contribution>();

                //var techs = await MvpApiService.GetContributionAreasAsync();
                //var vis = await MvpApiService.GetVisibilitiesAsync();
                //var types = await MvpApiService.GetContributionTypesAsync();

                //for (var i = 0; i < 100; i++)
                //{
                //    var techcat = techs[random.Next(0, techs.Count - 1)];
                //    var tech = techcat.ContributionAreas[random.Next(0, techcat.ContributionAreas.Count - 1)].ContributionTechnology;
                //    var id = random.Next(0, 1000);

                //    var contrib = new Contribution
                //    {
                //        AnnualQuantity = random.Next(0, 50),
                //        SecondAnnualQuantity = random.Next(0, 50),
                //        AnnualReach = random.Next(0, 50),
                //        StartDate = DateTime.Now.AddYears(-4).AddDays(random.Next(0, 4 * 365)),
                //        Description = "This is a non randomized description.",
                //        Title = $"My Activity #{id}",
                //        ContributionTechnology = tech[random.Next(0, tech.Count - 1)],
                //        ContributionType = types[random.Next(0, types.Count - 1)],
                //        Visibility = vis[random.Next(0, vis.Count - 1)],
                //        ReferenceUrl = "https://cataas.com/cat/says/hello%20world!",
                //        ContributionId = id
                //    };

                //    cont.Add(contrib);
                //}

                //var con = JsonConvert.SerializeObject(cont);

                if (await AuthService.SignInSilentAsync())
                {
                    FetchText = Resources.Translations.splash_contributionareas;
                    await MvpApiService.GetContributionAreasAsync();

                    FetchText = Resources.Translations.splash_contributiontypes;
                    await MvpApiService.GetContributionTypesAsync();

                    FetchText = Resources.Translations.splash_visibilities;
                    await MvpApiService.GetVisibilitiesAsync();

                    await GoToNextPage(true);
                }
                else
                {
                    // Fixed delay to show the animation on the frontend :$
                    FetchText = Resources.Translations.splash_gettingready;
                    await Task.Delay(2500);
                    await GoToNextPage(false);
                }
            }
            catch (Exception e)
            {
                AnalyticsService.Report(e);
                await GoToNextPage(false);
            }
        }

        public Task GoToNextPage(bool isAuthenticated)
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (isAuthenticated)
                {
                    AnalyticsService.Track("");
                    NavigationHelper.SetRootView(nameof(TabbedMainPage), false);
                }
                else
                {
                    AnalyticsService.Track("");
                    NavigationHelper.SetRootView(nameof(IntroPage));
                }
            });
        }

        public override Task OnAppearing()
        {
            PrefetchDataCommand.Execute(null);
            return base.OnAppearing();
        }
    }
}
