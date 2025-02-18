using System.Net.Http;
using System.Net.Http.Headers;

namespace ESPNelson.Model
{

    /// <summary>
    /// Initialisation du client HTTP au démarrage, définition de l'URL de base de StationnmentAPI et ajout des en-têtes JSON
    /// </summary>
    public static class APIHelper
    {
        public static HttpClient APIClient { get; private set; }

        static APIHelper()
        {
            InitializeClient(); // Initialisation automatique
        }

        public static void InitializeClient()
        {
            if (APIClient == null)
            {
                APIClient = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:7185/api/")
                };
                APIClient.DefaultRequestHeaders.Accept.Clear();
                APIClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }
    }
}
