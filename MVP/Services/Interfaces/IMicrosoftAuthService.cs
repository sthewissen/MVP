using System.Threading.Tasks;

namespace MVP.Services.Interfaces
{
    public interface IMicrosoftAuthService
    {
        Task SignInAsync(bool isRefresh = false);
        Task SignOutAsync();
    }
}
