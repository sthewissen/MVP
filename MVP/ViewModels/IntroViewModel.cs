using System;
using System.Threading.Tasks;
using MVP.Services;
using MVP.Services.Helpers;
using MvvmHelpers.Commands;

namespace MVP.ViewModels
{
    public class IntroViewModel : BaseViewModel
    {
        readonly AuthService _authService;

        public AsyncCommand SignInCommand { get; set; }

        public IntroViewModel()
        {
            _authService = new AuthService();
            SignInCommand = new AsyncCommand(SignIn);
        }

        async Task SignIn()
        {
            try
            {
                if (await _authService.SignInAsync())
                {
                    // TODO: Set MainPage to AppShell.xaml
                }
            }
            catch (Exception e)
            {
                e.LogException();

                // TODO: Inform the user too plz.
            }
        }
    }
}
