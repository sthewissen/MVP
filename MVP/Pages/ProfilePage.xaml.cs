using MVP.Services.Interfaces;
using Xamarin.Forms;
using System;

namespace MVP.Pages
{
    public partial class ProfilePage
    {
        public ProfilePage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        public void ScrollView_Scrolled(object sender, Xamarin.Forms.ScrolledEventArgs e)
        {
            appFrame.ShadowOpacity = e.ScrollY / 50 > 1 ? 1 : e.ScrollY / 50;
        }
    }
}
