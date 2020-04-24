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
    public class AddContributionPageModel : BasePageModel
    {
        readonly MvpApiService _mvpApiService;

        public IAsyncCommand BackCommand { get; set; }

        public List<ContributionType> ContributionTypes { get; set; } = new List<ContributionType>();
        public IList<MvvmHelpers.Grouping<string, ContributionTechnology>> GroupedContributionTechnologies { get; set; } = new List<MvvmHelpers.Grouping<string, ContributionTechnology>>();

        public AddContributionPageModel(MvpApiService mvpApiService)
        {
            _mvpApiService = mvpApiService;
            BackCommand = new AsyncCommand(() => Back());
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            LoadContributionTypes().SafeFireAndForget();
            LoadContributionAreas().SafeFireAndForget();
        }

        async Task LoadContributionTypes()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var types = await _mvpApiService.GetContributionTypesAsync().ConfigureAwait(false);

                if (types != null)
                {
                    ContributionTypes = types.OrderBy(x => x.Name).ToList();
                }
            }
        }

        async Task LoadContributionAreas()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var categories = await _mvpApiService.GetContributionAreasAsync().ConfigureAwait(false);

                if (categories != null)
                {
                    var result = new List<MvvmHelpers.Grouping<string, ContributionTechnology>>();

                    foreach (var item in categories.SelectMany(x => x.ContributionAreas))
                    {
                        result.Add(new MvvmHelpers.Grouping<string, ContributionTechnology>(item.AwardName, item.ContributionTechnology));

                    }

                    GroupedContributionTechnologies = result;
                }
            }
        }

        async Task Back()
        {
            await CoreMethods.PopPageModel().ConfigureAwait(false);
        }
    }
}
