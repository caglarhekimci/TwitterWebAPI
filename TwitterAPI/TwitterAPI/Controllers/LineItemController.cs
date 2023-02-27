using Microsoft.AspNetCore.Mvc;
using TwitterAdsAPIProject.Models;
using TwitterAPI.Models.CampaignManagement;
using TwitterAPI.Services;

namespace TwitterAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LineItemController : Controller
    {
        private readonly ITwitterAdsApiService _twitterAdsApiService;
        public LineItemController(ITwitterAdsApiService twitterAdsApiService)
        {
            _twitterAdsApiService = twitterAdsApiService;
        }

        // GET: api/line-items/followers
        [HttpGet("campaigns")]
        public async Task<ActionResult<List<LineItem>>> GetAccountAsync(string version)
        {

            string getAccountEndpoint = Accounts.GetAccount(version);
            //TwitterAdsApiService.updateUrl += TwitterAdsApiService._baseUrl + getAccountEndpoint;

            var lineItems = await _twitterAdsApiService.GetDataFromTwitter(getAccountEndpoint);
            if (lineItems == null)
            {
                return NotFound();
            }
            return lineItems;
        }
    }
}
