using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Extensions;
using MVP.Models;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using TinyMvvm;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class ContributionTypePickerViewModel : BaseViewModel
    {
        ContributionViewModel contribution;

        public IAsyncCommand<ContributionTypeViewModel> SelectContributionTypeCommand { get; set; }
        public IAsyncCommand RefreshDataCommand { get; set; }

        public List<ContributionTypeViewModel> ContributionTypes { get; set; } = new List<ContributionTypeViewModel>();

        public ContributionTypePickerViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
            RefreshDataCommand = new AsyncCommand(() => LoadContributionTypes(true));
            SelectContributionTypeCommand = new AsyncCommand<ContributionTypeViewModel>((x) => SelectContributionType(x));
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            // If a new contribution is coming in, the user created one from the URL
            // they had on the clipboard.
            if (NavigationParameter is ContributionViewModel contribution)
            {
                this.contribution = contribution;
            }

            LoadContributionTypes().SafeFireAndForget();
        }

        /// <summary>
        /// Loads the contribution types from cache.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        async Task LoadContributionTypes(bool force = false)
        {
            try
            {
                State = LayoutState.Loading;

                var types = await MvpApiService.GetContributionTypesAsync(force).ConfigureAwait(false);

                if (types == null)
                {
                    State = LayoutState.Error;
                    return;
                }

                ContributionTypes = types
                    .OrderBy(x => x.Name)
                    .Select(x => new ContributionTypeViewModel
                    {
                        ContributionType = x
                    }).ToList();

                // Editing mode
                if (contribution.ContributionType.Value != null)
                {
                    var selected = ContributionTypes.FirstOrDefault(x => x.ContributionType.Id == contribution.ContributionType.Value.Id);
                    selected.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                State = LayoutState.Error;
                AnalyticsService.Report(ex);
            }
            finally
            {
                if (State != LayoutState.Error)
                    State = ContributionTypes.Count > 0 ? LayoutState.None : LayoutState.Empty;
            }
        }

        /// <summary>
        /// Selects a specific contribution type for the contribution.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        async Task SelectContributionType(ContributionTypeViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in ContributionTypes)
                item.IsSelected = false;

            vm.IsSelected = true;

            //TODO: Replace by the back navigation version.
            contribution.ContributionType = new Validation.ValidatableObject<ContributionType> { Value = vm.ContributionType };

            AnalyticsService.Track("Contribution Type Picked",
                nameof(contribution.ContributionType),
                vm.ContributionType.Name);

            await BackAsync();
        }
    }
}
