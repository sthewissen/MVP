using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using MVP.Models;
using MVP.Services;
using Xamarin.Essentials;

namespace MVP.PageModels
{
    public class WizardVisibilityPageModel : BasePageModel
    {
        readonly MvpApiService _mvpApiService;
        Visibility _selectedVisibility;
        Contribution _contribution;

        public IAsyncCommand BackCommand { get; set; }
        public IAsyncCommand<Contribution> NextCommand { get; set; }

        public IList<Visibility> Visibilities { get; set; } = new List<Visibility>();

        public Visibility SelectedVisibility
        {
            get => _selectedVisibility;
            set
            {
                _selectedVisibility = value;

                if (value != null)
                {
                    _contribution.Visibility = value;
                    NextCommand.Execute(_contribution);
                }
            }
        }

        public WizardVisibilityPageModel(MvpApiService mvpApiService)
        {
            _mvpApiService = mvpApiService;
            BackCommand = new AsyncCommand(() => Back());
            NextCommand = new AsyncCommand<Contribution>((contribution) => Next(contribution));
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            if (initData is Contribution contribution)
            {
                _contribution = contribution;
            }

            LoadVisibilities().SafeFireAndForget();
        }

        async Task LoadVisibilities()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var result = await _mvpApiService.GetVisibilitiesAsync().ConfigureAwait(false);

                if (result != null)
                {
                    Visibilities = result.ToList();

                    // Editing mode
                    if (_contribution.Visibility != null)
                    {
                        _selectedVisibility = result
                            .FirstOrDefault(x => x.Id == _contribution.Visibility.Id);

                        RaisePropertyChanged(nameof(SelectedVisibility));
                    }
                }
            }
        }

        async Task Back()
        {
            await CoreMethods.PopPageModel(modal: false, animate: false).ConfigureAwait(false);
        }

        async Task Next(Contribution contribution)
        {
            await CoreMethods.PushPageModel<WizardAmountsPageModel>(data: contribution, modal: false, animate: false).ConfigureAwait(false);
        }
    }
}
