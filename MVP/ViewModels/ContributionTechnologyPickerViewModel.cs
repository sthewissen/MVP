﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Extensions;
using MVP.Helpers;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using TinyMvvm;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class ContributionTechnologyPickerViewModel : BaseViewModel
    {
        ContributionViewModel contribution;
        IReadOnlyList<Models.ContributionCategory> allCategories = new List<Models.ContributionCategory>();

        public string SearchText { get; set; } = string.Empty;
        public ICommand SelectContributionTechnologyCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public IList<Grouping<string, ContributionTechnologyViewModel>> GroupedContributionTechnologies { get; set; } = new List<Grouping<string, ContributionTechnologyViewModel>>();

        public ContributionTechnologyPickerViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
            SearchCommand = new Command(() => PopulateList());
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

        async Task LoadContributionAreas()
        {
            try
            {
                State = LayoutState.Loading;

                allCategories = await MvpApiService.GetContributionAreasAsync().ConfigureAwait(false);

                PopulateList();
            }
            finally
            {
                State = LayoutState.None;
            }
        }

        void PopulateList()
        {
            if (allCategories == null)
                return;

            var result = new List<Grouping<string, ContributionTechnologyViewModel>>();

            foreach (var item in allCategories.SelectMany(x => x.ContributionAreas))
            {
                var data = item.ContributionTechnology
                                .Where(x => x.Name.ToLowerInvariant().Contains(SearchText.ToLowerInvariant()) || string.IsNullOrEmpty(SearchText))
                                .Select(x => new ContributionTechnologyViewModel() { ContributionTechnology = x });

                if (data.Any())
                    result.Add(new Grouping<string, ContributionTechnologyViewModel>(item.AwardName, data));
            }

            GroupedContributionTechnologies = result;

            // Editing mode
            if (contribution.ContributionTechnology.Value == null)
                return;

            var selected = result
                .SelectMany(x => x)
                .FirstOrDefault(x => x.ContributionTechnology.Id == contribution.ContributionTechnology.Value.Id);

            selected.IsSelected = true;
        }

        void SelectContributionTechnology(ContributionTechnologyViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in GroupedContributionTechnologies)
                foreach (var tech in item)
                    tech.IsSelected = false;

            vm.IsSelected = true;

            //TODO: Replace by the back navigation version.
            contribution.ContributionTechnology.Value = vm.ContributionTechnology;
        }

        public async override Task Back()
            => await NavigationHelper.BackAsync(GroupedContributionTechnologies.SelectMany(x => x).FirstOrDefault(x => x.IsSelected)?.ContributionTechnology);
    }
}