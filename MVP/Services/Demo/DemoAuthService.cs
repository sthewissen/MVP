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
            => Task.FromResult(Settings.IsUsingDemoAccount);

        public Task<bool> SignInAsync()
            => Task.FromResult(Settings.IsUsingDemoAccount);

        public Task<bool> SignOutAsync()
        {
            (Application.Current as App).SwitchDemoMode(false);
            Settings.IsUsingDemoAccount = false;
            return Task.FromResult(true);
        }
    }
}
