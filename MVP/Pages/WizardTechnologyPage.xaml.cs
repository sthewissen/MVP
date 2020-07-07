using MVP.Services.Interfaces;

namespace MVP.Pages
{
    public partial class WizardTechnologyPage
    {
        public WizardTechnologyPage(IAnalyticsService analyticsService)
            : base(analyticsService) => InitializeComponent();
    }
}
