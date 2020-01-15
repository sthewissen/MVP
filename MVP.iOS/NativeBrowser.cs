using MVP.iOS;
using Xamarin.Forms;
using UIKit;
using Foundation;
using SafariServices;
using System.Threading.Tasks;
using Xamarin.Essentials;

[assembly: Dependency(typeof(NativeBrowser))]
namespace MVP.iOS
{
    public class NativeBrowser : INativeBrowser
    {
        UIViewController rootViewController;
        SFSafariViewController safari;

        Task<BrowserResult> INativeBrowser.LaunchBrowserAsync(string url)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            AppDelegate.CallbackHandler = async (response) =>
            {
                await safari.DismissViewControllerAsync(true);

                tcs.SetResult(new BrowserResult
                {
                    Response = response,
                    ResultType = BrowserResultType.Success
                });
            };

            rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            safari = new SFSafariViewController(new NSUrl(url));
            rootViewController.PresentViewController(safari, true, null);

            return tcs.Task;
        }
    }

}
