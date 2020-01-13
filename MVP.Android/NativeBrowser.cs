using Android.App;
using Xamarin.Forms;
using System.Threading.Tasks;
using MVP.Droid;

[assembly: Dependency(typeof(NativeBrowser))]
namespace MVP.Droid
{
    public class NativeBrowser : INativeBrowser
    {
        async Task<BrowserResult> INativeBrowser.LaunchBrowserAsync(string url)
        {
            var browser = new ChromeCustomTabsWebView((Activity)MainActivity.Instance);
            return await browser.InvokeAsync(url);
        }
    }
}
