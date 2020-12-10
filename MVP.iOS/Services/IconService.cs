using System.Threading.Tasks;
using MVP.iOS.Services;
using MVP.Services.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(IconService))]
namespace MVP.iOS.Services
{
    public class IconService : IIconService
    {
        public async Task SwitchAppIcon(string iconName)
        {
            if (UIKit.UIApplication.SharedApplication.SupportsAlternateIcons)
            {
                await UIKit.UIApplication.SharedApplication.SetAlternateIconNameAsync(iconName);
            }
        }
    }
}