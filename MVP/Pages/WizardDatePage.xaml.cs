using System;
using System.Collections.Generic;
using MVP.Services.Interfaces;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class WizardDatePage
    {
        public WizardDatePage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();
    }
}
