using System.Threading.Tasks;
using Acr.UserDialogs;

namespace MVP.Services
{
    public static class DialogService
    {
        public static Task AlertAsync(string message, string title, string buttonLabel)
            => UserDialogs.Instance.AlertAsync(message, title, buttonLabel);

        public static Task<string> ActionSheetAsync(string message, string cancel = null, string destructive = null, params string[] buttons)
            => UserDialogs.Instance.ActionSheetAsync(message, cancel, destructive, null, buttons);

        public static Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null)
            => UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
    }
}
