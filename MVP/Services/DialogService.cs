using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using MVP.Services.Interfaces;

namespace MVP.Services
{
    public class DialogService : IDialogService
    {
        public Task AlertAsync(string message, string title, string buttonLabel)
            => UserDialogs.Instance.AlertAsync(message, title, buttonLabel);

        public Task<string> ActionSheetAsync(string message, string cancel = null, string destructive = null, params string[] buttons)
            => UserDialogs.Instance.ActionSheetAsync(message, cancel, destructive, null, buttons);

        public Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null)
            => UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
    }
}
