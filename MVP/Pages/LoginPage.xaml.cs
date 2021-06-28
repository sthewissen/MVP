using System;
using MVP.Services.Interfaces;
using Xamarin.Forms;

namespace MVP.Pages
{
    public partial class LoginPage
    {
        public LoginPage(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            InitializeComponent();
        } 
    }
}
