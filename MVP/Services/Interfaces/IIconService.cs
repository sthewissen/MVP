using System;
using System.Threading.Tasks;

namespace MVP.Services.Interfaces
{
    public interface IIconService
    {
        Task SwitchAppIcon(string iconName);
    }
}
