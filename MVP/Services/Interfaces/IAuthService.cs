using System.Threading.Tasks;

namespace MVP.Services
{
    public interface IAuthService
    {
        Task<bool> SignInAsync();
        Task<bool> SignInSilentAsync();
        Task<bool> SignOutAsync();
    }
}