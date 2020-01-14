using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVP.Services.Helpers
{
    public static class Extensions
    {
        public static void LogException(this Exception ex)
        {
            // Log to AppCenter.
        }

        public static Task LogExceptionAsync(this Exception ex)
        {
            // Log to AppCenter.
            return Task.FromResult(0);
        }
    }
}
