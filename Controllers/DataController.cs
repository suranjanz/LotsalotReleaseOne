
using System.Reflection.Metadata;

namespace LotsalotTestApi.Controllers
{

    //var response = await client.GetAsync($"https://lottery.merseyworld.com/cgi-bin/lottery?days=2&Machine=Z&Ballset=0&order=0&show=1&year={parameter}&display=CSV");

    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public DataController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [HttpGet("{parameter}")]
        public async Task<string> GetExternalData(string parameter)
        {
            try
            {
                var client = _clientFactory.CreateClient();
		
        	client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        	client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

                // Explicitly limit how long the server will wait for the external URL (e.g., 10 seconds)
                client.Timeout = TimeSpan.FromSeconds(10);

                var response = await client.GetAsync($"https://lottery.merseyworld.com/cgi-bin/lottery?days=2&Machine=Z&Ballset=0&order=0&show=1&year={parameter}&display=CSV");
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(content)) return "Error : No Data from Controller";
                    if (content.Contains("Lotsalot")) return "Error : Index Returned From Controller";
                    return content;
                }

                return $"Error: External API returned status {response.StatusCode}";
            }
            catch (TaskCanceledException)
            {
                // This specific exception is thrown by HttpClient when the configured timeout expires
                return "Error: External API connection timed out.";
            }
            catch (Exception ex)
            {
                return $"Error: Server external connection failed - {ex.Message}";
            }
        }
    }


}
