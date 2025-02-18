using System.Net.Http;
using System.Net.Http.Headers;

namespace BornePaiement.Models
{
    public static class APIHelper
    {
        public static HttpClient APIClient { get; private set; }

        public static void InitializeClient()
        {
            APIClient = new HttpClient
            {
                BaseAddress = new Uri("https://stationnementapi.com/api/")
            };
            APIClient.DefaultRequestHeaders.Accept.Clear();
            APIClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
