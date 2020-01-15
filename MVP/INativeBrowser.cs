using System.Threading.Tasks;

namespace MVP
{
    public interface INativeBrowser
    {
        Task<BrowserResult> LaunchBrowserAsync(string url);
    }

    public class BrowserResult
    {
        public BrowserResultType ResultType { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }
        public bool IsError => !string.IsNullOrEmpty(Error);
    }

    public enum BrowserResultType
    {
        Success,
        HttpError,
        UserCancel,
        Timeout,
        UnknownError
    }
}
