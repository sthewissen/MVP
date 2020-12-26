using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP.Extensions;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using TinyNavigationHelper;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;

namespace MVP.ViewModels
{
    public class VisibilityPickerViewModel : BaseViewModel
    {
        ContributionViewModel contribution;

        public IAsyncCommand<VisibilityViewModel> SelectVisibilityCommand { get; }
        public IAsyncCommand RefreshDataCommand { get; set; }

        public IList<VisibilityViewModel> Visibilities { get; set; } = new List<VisibilityViewModel>();

        public VisibilityPickerViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
            RefreshDataCommand = new AsyncCommand(() => LoadVisibilities(true));
            SelectVisibilityCommand = new AsyncCommand<VisibilityViewModel>((x) => SelectVisibility(x));
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            if (NavigationParameter is ContributionViewModel contrib)
            {
                contribution = contrib;
            }

            LoadVisibilities().SafeFireAndForget();
        }

        /// <summary>
        /// Selects a visibility for the contribuion being created/edited.
        /// </summary>
        async Task SelectVisibility(VisibilityViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in Visibilities)
                item.IsSelected = false;

            vm.IsSelected = true;

            //TODO: Replace by the back navigation version in TinyMvvm 3.0.
            contribution.Visibility.Value = vm.Visibility;

            AnalyticsService.Track("Visibility Picked", nameof(vm.Visibility), vm.Visibility.Description);

            await NavigationHelper.BackAsync();
        }

        /// <summary>
        /// Loads visibilities from cache.
        /// </summary>
        async Task LoadVisibilities(bool force = false)
        {
            try
            {
                State = LayoutState.Loading;

                var visibilities = await MvpApiService.GetVisibilitiesAsync(force).ConfigureAwait(false);

                if (visibilities == null)
                {
                    State = LayoutState.Error;
                    return;
                }

                Visibilities = visibilities.Select(x => new VisibilityViewModel() { Visibility = x }).ToList();

                // Editing mode
                if (contribution.Visibility.Value != null)
                {
                    var selectedVisibility = Visibilities.FirstOrDefault(x => x.Visibility.Id == contribution.Visibility.Value.Id);
                    selectedVisibility.IsSelected = true;
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
                    State = Visibilities.Count > 0 ? LayoutState.None : LayoutState.Empty;
            }
        }

        public async override Task Back()
            => await NavigationHelper.BackAsync(); // TODO: TinyMVVM 3.0 - Visibilities.FirstOrDefault(x => x.IsSelected)?.Visibility);
    }
}
