using System;
using System.Windows.Input;
using MVP.Services;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            SignInCommand = new Command(async () =>
            {
                var auth = new MicrosoftAuthService();
                await auth.SignInAsync();
            });
            SignOutCommand = new Command(async () =>
            {
                var auth = new MicrosoftAuthService();
                await auth.SignOutAsync();
            });
        }

        public ICommand SignInCommand { get; }
        public ICommand SignOutCommand { get; }
    }
}