using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Models;
using MVP.Pages;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using MvvmHelpers;
using TinyMvvm;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class ContributionTechnologyPickerViewModel : BaseViewModel
    {
        ContributionViewModel contribution;

        public bool IsEditing { get; set; }
        public ICommand SelectContributionTechnologyCommand { get; set; }

        public IList<Grouping<string, ContributionTechnologyViewModel>> GroupedContributionTechnologies { get; set; } = new List<Grouping<string, ContributionTechnologyViewModel>>();

        public ContributionTechnologyPickerViewModel(IAnalyticsService analyticsService, IAuthService authService,
            IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, authService, dialogService, navigationHelper)
        {
            SelectContributionTechnologyCommand = new Command<ContributionTechnologyViewModel>((x) => SelectContributionTechnology(x));
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is ContributionViewModel contribution)
            {
                this.contribution = contribution;
            }

            LoadContributionAreas().SafeFireAndForget();
        }

        public async override Task Back()
            => await NavigationHelper.BackAsync(GroupedContributionTechnologies.SelectMany(x => x).FirstOrDefault(x => x.IsSelected)?.ContributionTechnology);

        async Task LoadContributionAreas()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var categories = await MvpApiService.GetContributionAreasAsync().ConfigureAwait(false);

                if (categories != null)
                {
                    var result = new List<Grouping<string, ContributionTechnologyViewModel>>();

                    foreach (var item in categories.SelectMany(x => x.ContributionAreas))
                    {
                        result.Add(
                            new Grouping<string, ContributionTechnologyViewModel>(item.AwardName,
                            item.ContributionTechnology.Select(x => new ContributionTechnologyViewModel()
                            {
                                ContributionTechnology = x
                            }))
                        );
                    }

                    GroupedContributionTechnologies = result;

                    // Editing mode
                    if (contribution.ContributionTechnology.Value != null)
                    {
                        var selected = result
                            .SelectMany(x => x)
                            .FirstOrDefault(x => x.ContributionTechnology.Id == contribution.ContributionTechnology.Value.Id);

                        selected.IsSelected = true;
                    }
                }
            }
        }

        void SelectContributionTechnology(ContributionTechnologyViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in GroupedContributionTechnologies)
                foreach (var tech in item)
                    tech.IsSelected = false;

            vm.IsSelected = true;
            contribution.ContributionTechnology.Value = vm.ContributionTechnology;
        }
    }
}
