using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/csv/{year}", async (int year, IHttpClientFactory factory) =>
{
    try
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

        // 🔽 CHANGE THIS TO YOUR ACTUAL EXTERNAL URL
        string externalUrl = $"https://your-external-site.com/data/{year}.csv";

        var csvData = await client.GetStringAsync(externalUrl);
        return Results.Text(csvData);
    }
    catch (Exception ex)
    {
        // This returns the full error as plain text – you'll see it in your browser
        return Results.Text($"ERROR: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
    }
});

app.MapFallbackToFile("index.html");

app.Run();