using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using TwitterAdsAPIProject.Models;
using TwitterAPI.Controllers;
using TwitterAPI.Models.CampaignManagement;

namespace TwitterAPI.Services
{
    public class TwitterAdsApiService : ITwitterAdsApiService
    {
        private static HttpClient? _httpClient;
        private static string _baseUrl;
        private static string _consumerKey;
        private static string _consumerSecret;
        private static string _accessToken;
        private static string _tokenSecret;


        public TwitterAdsApiService(IConfiguration config)
        {
            _baseUrl = config.GetValue<string>("TwitterAPIConnections:BaseUrl") ?? throw new ArgumentException("BaseUrl not found in configuration");
            _consumerKey = config.GetValue<string>("TwitterAPIConnections:consumer_key") ?? throw new ArgumentException("consumer_key not found in configuration");
            _consumerSecret = config.GetValue<string>("TwitterAPIConnections:consumer_secret") ?? throw new ArgumentException("consumer_secret not found in configuration");
            _accessToken = config.GetValue<string>("TwitterAPIConnections:access_token") ?? throw new ArgumentException("access_token not found in configuration");
            _tokenSecret = config.GetValue<string>("TwitterAPIConnections:token_secret") ?? throw new ArgumentException("token_secret not found in configuration");
            _httpClient = HttpClient;
        }

        public AuthenticationHeaderValue GenerateAuthHeaderValue(string path)
        {
            var oauthNonce = Guid.NewGuid().ToString();
            var oauthTimestamp = ((int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds).ToString();

            //1.Convert the HTTP Method to uppercase and set the output string equal to this value.
            string Signature_Base_String = "GET";
            Signature_Base_String = Signature_Base_String.ToUpper();

            //2.Append the ‘&’ character to the output string.
            Signature_Base_String += "&";

            //3.Percent encode the URL and append it to the output string.
            string PercentEncodedURL = Uri.EscapeDataString(_baseUrl + path);
            Signature_Base_String += PercentEncodedURL;

            //4.Append the ‘&’ character to the output string.
            Signature_Base_String += "&";

            //5.append OAuth parameter string to the output string.
            var parameters = new SortedDictionary<string, string>

             {
             {"oauth_consumer_key", _consumerKey},
             {"oauth_token", _accessToken },
             {"oauth_signature_method", "HMAC-SHA1"},
             {"oauth_timestamp", oauthTimestamp},
             {"oauth_nonce", oauthNonce},
             {"oauth_version", "1.0"}
             };
            //6. sort the parameter by key
            var sortedParameters = from p in parameters
                                   orderby p.Key ascending
                                   select p; bool first = true;
            string keyValuePair = "";
            //7. percent encode key and value and append to the output string
            foreach (KeyValuePair<string, string> elt in sortedParameters)
            {
                if (first)
                {
                    keyValuePair = Uri.EscapeDataString(elt.Key) + "=" + Uri.EscapeDataString(elt.Value);
                    first = false;
                }
                else
                {
                    keyValuePair += "&" + Uri.EscapeDataString(elt.Key) + "=" + Uri.EscapeDataString(elt.Value);
                }
            }
            Signature_Base_String += Uri.EscapeDataString(keyValuePair); string signatureKey = _consumerSecret + "&" + _tokenSecret;
            HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));
            byte[] signatureBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(Signature_Base_String));
            string signature = Convert.ToBase64String(signatureBytes);
            signature = HttpUtility.UrlEncode(signature);
            parameters.Add("oauth_signature", signature);
            return new AuthenticationHeaderValue("OAuth", string.Join(", ", parameters.Select(x => x.Key + "=\"" + x.Value + "\"")));
        }

        private static HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }
                return _httpClient;

            }

        }

        public async Task<List<LineItem>> GetDataFromTwitter(string endpoint)
        {
            //_httpClient.DefaultRequestHeaders.Authorization = GenerateAuthHeaderValue(endpoint);
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + endpoint);
            request.Headers.Authorization = GenerateAuthHeaderValue(endpoint);
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            //string x= _baseUrl+ endpoint; //control!
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error: " + response.StatusCode + " - " + response.ReasonPhrase);
                return null;
            }

            string responseJson = await response.Content.ReadAsStringAsync();
            dynamic responseData = JsonConvert.DeserializeObject<dynamic>(responseJson);
            if (responseData == null) return null;

            List<LineItem> lineItemList = new();
            foreach (var item in responseData.data)
            {
                LineItem lineItem = new LineItem
                {
                    Metrics = item.ToObject<Dictionary<string, object>>()
                };
                lineItemList.Add(lineItem);
            }

            return lineItemList;
        }
    }
}
