using System;
using System.Collections.Generic;
using MVP.Services.Interfaces;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class WizardActivityTypePage
    {
        public WizardActivityTypePage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();
    }
}
