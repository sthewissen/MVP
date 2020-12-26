using System.Threading.Tasks;
using MVP.Services.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MVP.Services.Demo
{
    public class DemoAuthService : IAuthService
    {
        public DemoAuthService()
        {
        }

        public Task<bool> SignInSilentAsync()
            => Task.FromResult(Preferences.Get(Settings.IsUsingDemoAccount, Settings.IsUsingDemoAccountDefault));

        public Task<bool> SignInAsync()
            => Task.FromResult(Preferences.Get(Settings.IsUsingDemoAccount, Settings.IsUsingDemoAccountDefault));

        public Task<bool> SignOutAsync()
        {
            (Application.Current as App).SwitchDemoMode(false);
            Preferences.Set(Settings.IsUsingDemoAccount, false);
            return Task.FromResult(true);
        }
    }
}
