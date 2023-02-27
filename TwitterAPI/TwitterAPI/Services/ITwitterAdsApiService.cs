using TwitterAdsAPIProject.Models;

namespace TwitterAPI.Services
{
    public interface ITwitterAdsApiService
    {
        Task<List<LineItem>> GetDataFromTwitter(string endpoint);
    }
}