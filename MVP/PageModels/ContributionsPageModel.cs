using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using MVP.Extensions;
using MVP.Models;
using MVP.Services;
using MvvmHelpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.PageModels
{
    public class ContributionsPageModel : BasePageModel
    {
        public IAsyncCommand OpenProfileCommand { get; set; }
        public IAsyncCommand<Contribution> OpenContributionCommand { get; set; }
        public IAsyncCommand OpenAddContributionCommand { get; set; }

        public ContributionsPageModel()
        {
            OpenProfileCommand = new AsyncCommand(OpenProfile);
            OpenContributionCommand = new AsyncCommand<Contribution>(OpenContribution);
            OpenAddContributionCommand = new AsyncCommand(OpenAddContribution);
        }

        public async override void Init(object initData)
        {
            base.Init(initData);
        }

        async Task OpenProfile()
        {
            await CoreMethods.PushPageModel<ProfilePageModel>();
        }

        async Task OpenAddContribution()
        {
            await CoreMethods.PushPageModel<ProfilePageModel>();
        }

        async Task OpenContribution(Contribution contribution)
        {
            await CoreMethods.PushPageModel<ProfilePageModel>();
        }
    }
}
