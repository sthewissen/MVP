using System;
using System.Threading.Tasks;
using Refit;

namespace MVP.Services.Interfaces
{
    [Headers("User-Agent: MVP App", "Authorization: Bearer", "Ocp-Apim-Subscription-Key: cd3b2e5d2edc43718ae46a4dfd627323")]
    public interface IMvpApi
    {
        [Delete("/contributions")]
        Task DeleteContribution(int id);
    }
}
