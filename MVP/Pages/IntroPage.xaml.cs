using MVP.Models;
using MVP.Services.Interfaces;
using MVP.ViewModels;
using Xamarin.CommunityToolkit.Converters;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class IntroPage
    {
        public IntroPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();
    }
}
