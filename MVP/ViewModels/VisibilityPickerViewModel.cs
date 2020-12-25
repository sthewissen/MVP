using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Extensions;
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
    public class VisibilityPickerViewModel : BaseViewModel
    {
        ContributionViewModel contribution;

        public IAsyncCommand<VisibilityViewModel> SelectVisibilityCommand { get; }

        public IList<VisibilityViewModel> Visibilities { get; set; } = new List<VisibilityViewModel>();

        public VisibilityPickerViewModel(IAnalyticsService analyticsService, INavigationHelper navigationHelper)
            : base(analyticsService, navigationHelper)
        {
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

        async Task SelectVisibility(VisibilityViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in Visibilities)
                item.IsSelected = false;

            vm.IsSelected = true;

            //TODO: Replace by the back navigation version.
            contribution.Visibility.Value = vm.Visibility;

            await NavigationHelper.BackAsync();
        }

        public async override Task Back()
            => await NavigationHelper.BackAsync(); // TODO: TinyMVVM 3.0 - Visibilities.FirstOrDefault(x => x.IsSelected)?.Visibility);

        async Task LoadVisibilities()
        {
            try
            {
                State = LayoutState.Loading;

                var visibilities = await MvpApiService.GetVisibilitiesAsync().ConfigureAwait(false);

                if (visibilities != null)
                {
                    Visibilities = visibilities.Select(x => new VisibilityViewModel() { Visibility = x }).ToList();

                    // Editing mode
                    if (contribution.Visibility.Value != null)
                    {
                        var selectedVisibility = Visibilities.FirstOrDefault(x => x.Visibility.Id == contribution.Visibility.Value.Id);
                        selectedVisibility.IsSelected = true;
                    }
                }
            }
            finally
            {
                State = Visibilities.Count > 0 ? LayoutState.None : LayoutState.Empty;
            }
        }
    }
}
