using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MVP.Extensions;
using MVP.Services.Interfaces;
using MVP.ViewModels.Data;
using TinyMvvm;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class VisibilityPickerViewModel : BaseViewModel
    {
        ContributionViewModel contribution;

        public ICommand SelectVisibilityCommand { get; }

        public IList<VisibilityViewModel> Visibilities { get; set; } = new List<VisibilityViewModel>();

        public VisibilityPickerViewModel(IAnalyticsService analyticsService, IDialogService dialogService, INavigationHelper navigationHelper)
            : base(analyticsService, dialogService, navigationHelper)
        {
            SelectVisibilityCommand = new Command<VisibilityViewModel>((x) => SelectVisibility(x));
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

        void SelectVisibility(VisibilityViewModel vm)
        {
            if (vm == null)
                return;

            foreach (var item in Visibilities)
                item.IsSelected = false;

            vm.IsSelected = true;

            //TODO: Replace by the back navigation version.
            contribution.Visibility.Value = vm.Visibility;
        }

        public async override Task Back()
            => await NavigationHelper.BackAsync(Visibilities.FirstOrDefault(x => x.IsSelected)?.Visibility);

        async Task LoadVisibilities()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var result = await MvpApiService.GetVisibilitiesAsync().ConfigureAwait(false);

                if (result != null)
                {
                    Visibilities = result.Select(x => new VisibilityViewModel() { Visibility = x }).ToList();

                    // Editing mode
                    if (contribution.Visibility.Value != null)
                    {
                        var selectedVisibility = Visibilities.FirstOrDefault(x => x.Visibility.Id == contribution.Visibility.Value.Id);
                        selectedVisibility.IsSelected = true;
                    }
                }
            }
        }
    }
}
