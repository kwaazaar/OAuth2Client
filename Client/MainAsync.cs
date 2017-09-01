using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Client
{
    public class MainAsync
    {
        public static void Run() => new MainAsync().RunAsync().GetAwaiter().GetResult();

        public async Task RunAsync()
        {
            // Configuratie
            var authority = "https://login.microsoftonline.com/roberttekaatinfosupport.onmicrosoft.com";
            var clientId = "e932c2f4-3362-44d8-b63e-0e05d73faf9a";
            var clientSecret = "O5gk/M84I4R2L/rQoom6yX5kADtO6L406HBsmd+9GWU=";
            var resource = "https://apimgt"; // Resources kunnen per api worden gedefinieerd of bv voor de gehele omgeving in 1x. Indien slechts 1 AD voor alle OTAP omgevingen, dan aparte resources voor P en de rest.

            // ADAL gebruiken voor afhandelen van token requests. Kan makkelijk zelf, maar ADAL regelt ook caching/reuse van token en auto-refresh
            var authContext = new AuthenticationContext(authority, true, TokenCache.DefaultShared);
            var clientCredential = new ClientCredential(clientId, clientSecret);

            // HttpClient initialiseren (nooit in using ivm tcp keep-alive support)
            var client = new HttpClient { BaseAddress = new Uri("https://secured.azure-api.net") }; // Base-address van API-management api, niet de daadwerkelijke backend api!
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "eaa99792ca4147858ebc04c99c6abe90"); // Deze moet altijd, zodat API Management onderscheid kan maken tussen afnemers van de API

            // Meerdere calls doen
            for (int i = 0; i < 5; i++)
            {
                // Token ophalen (alleen eerste keer wordt deze daadwerkelijk opgehaald), en aan header toevoegen
                var tokenResult = await authContext.AcquireTokenAsync(resource, clientCredential);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenResult.AccessTokenType, tokenResult.AccessToken);

                // Api aanroepen
                var res = await client.GetAsync("/api/values");
                var content = await res.Content?.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                    Console.WriteLine($"HTTP {(int)res.StatusCode}: {content}");
                else
                    Console.WriteLine($"Json-response: {content}");
            }
        }
    }
}
