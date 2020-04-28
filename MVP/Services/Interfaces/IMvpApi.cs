using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP.Models;
using Refit;

namespace MVP.Services.Interfaces
{
    [Headers("User-Agent: MVP App",
        "Authorization: Bearer",
        "Ocp-Apim-Subscription-Key: cd3b2e5d2edc43718ae46a4dfd627323",
        "Content-Type: application/json")]
    public interface IMvpApi
    {
        [Get("/profile")]
        Task<Profile> GetProfile();

        [Get("/profile/photo")]
        Task<string> GetProfileImage();

        [Get("/contributions/{offset}/{limit}")]
        Task<ContributionList> GetContributions(int offset, int limit);

        [Post("/contributions")]
        Task<Contribution> AddContribution([Body]Contribution contribution);

        [Put("/contributions")]
        Task UpdateContribution([Body]Contribution contribution);

        [Delete("/contributions")]
        Task DeleteContribution(int id);

        [Get("/contributions/contributiontypes")]
        Task<List<ContributionType>> GetContributionTypes();

        [Get("/contributions/contributionareas")]
        Task<List<ContributionCategory>> GetContributionAreas();

        [Get("/contributions/sharingpreferences")]
        Task<List<Visibility>> GetVisibilities();

        [Get("/onlineidentities")]
        Task<List<OnlineIdentity>> GetOnlineIdentities();

        [Post("/onlineidentities")]
        Task<OnlineIdentity> AddOnlineIdentity([Body]OnlineIdentity identity);

        [Delete("/onlineidentities")]
        Task DeleteOnlineIdentity(int id);

        [Get("/awardconsideration/getcurrentquestions")]
        Task<List<AwardConsiderationQuestion>> GetAwardConsiderationQuestions();

        [Get("/awardconsideration/getanswers")]
        Task<List<AwardConsiderationAnswer>> GetAwardConsiderationAnswers();

        [Post("/awardconsideration/saveanswers")]
        Task<List<AwardConsiderationAnswer>> SaveAwardConsiderationAnswers([Body] IEnumerable<AwardConsiderationAnswer> answers);

        [Post("/awardconsideration/submitanswers")]
        Task SubmitAwardConsiderationAnswers();
    }
}