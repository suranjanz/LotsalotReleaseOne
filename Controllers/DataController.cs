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

            // Explicitly limit how long the server will wait for the external URL (e.g., 10 seconds)
            client.Timeout = TimeSpan.FromSeconds(10);

            var response = await client.GetAsync($"https://externalurl.com{parameter}");
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
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
