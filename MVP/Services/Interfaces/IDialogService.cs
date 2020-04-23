using System;
using System.Threading.Tasks;

namespace MVP.Services.Interfaces
{
    public interface IDialogService
    {
        Task AlertAsync(string message, string title, string buttonLabel);
        Task<string> ActionSheetAsync(string message, string cancel = null, string destructive = null, params string[] buttons);
        Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null);
    }
}
