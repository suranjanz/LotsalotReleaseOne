//this version avoids CORS and the Release version output folder should be copy pasted here in this project folder

 
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

var app = builder.Build();
//app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapGet("/api/csv/{year}", async (int year, IHttpClientFactory factory) =>
{
    try
    {
        var client = factory.CreateClient();

        // Add browser-like headers to avoid 403 Forbidden
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        );
        client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");

        // Your external CSV URL – replace with the real one
        string externalUrl = $"https://lottery.merseyworld.com/cgi-bin/lottery?days=2&Machine=Z&Ballset=0&order=0&show=1&year={year}&display=CSV";

        var csvData = await client.GetStringAsync(externalUrl);
        return Results.Text(csvData);
    }
    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
    {
        return Results.Text($"error : The external server returned 403 Forbidden. The URL may be blocked.");
    }
    catch (Exception ex)
    {
        return Results.Text($"error : {ex.Message}");
    }
});




//app.MapGet("/", () => Results.Text("...API Loaded And Waiting..."));//dummy page better than error page
app.MapFallbackToFile("index.html");


app.Run();