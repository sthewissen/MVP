using System;
using System.Collections.Generic;
using MVP.Services.Interfaces;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class AppIconPickerPage
    {
        public AppIconPickerPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        void CollectionView_Scrolled(object sender, Xamarin.Forms.ItemsViewScrolledEventArgs e)
        {
            appFrame.ShadowOpacity = e.VerticalOffset / 50 > 1 ? 1 : e.VerticalOffset / 50;
        }
    }
}
