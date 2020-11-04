using System;
using System.Collections.Generic;
using MVP.Services.Interfaces;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class WizardUrlPage
    {
        public WizardUrlPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();

        protected override void OnAppearing()
        {
            base.OnAppearing();
            entry.Focus();
        }
    }
}
