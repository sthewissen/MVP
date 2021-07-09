using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MVP.Services.Interfaces;
using Xamarin.CommunityToolkit.Converters;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class ContributionFormPage
    {
        public ContributionFormPage(IAnalyticsService analyticsService)
           : base(analyticsService)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.Android &&
                ToolbarItems.Any(x => x.Priority < 0))
            {
                var toolbarItems = ToolbarItems.FirstOrDefault(x => x.Priority < 0);
                ToolbarItems.Remove(toolbarItems);
            }
        }
    }
}
