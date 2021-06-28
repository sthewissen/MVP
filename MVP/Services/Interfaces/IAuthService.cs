using System.Threading.Tasks;

namespace MVP.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SignInAsync();
        Task<bool> SignInSilentAsync();
        Task<bool> SignOutAsync();
        Task<string> RequestAuthorizationAsync(string authCode, bool isRefresh = false);
    }
}