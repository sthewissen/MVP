using System.Collections.Generic;
using System.Globalization;
using MVP.Services.Interfaces;
using Xamarin.CommunityToolkit.Converters;

namespace MVP.Pages
{
    public partial class ContributionFormPage
    {
        public ContributionFormPage(IAnalyticsService analyticsService)
           : base(analyticsService) => InitializeComponent();
    }
}
