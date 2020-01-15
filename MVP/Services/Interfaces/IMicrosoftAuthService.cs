using System.Threading.Tasks;

namespace MVP.Services.Interfaces
{
    public interface IMicrosoftAuthService
    {
        Task<string> SignInAsync(bool isRefresh = false);
        Task SignOutAsync();
    }
}
