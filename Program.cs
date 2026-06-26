using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Standard API services + HttpClientFactory for external URL calls
builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

// 1. GLOBAL UNHANDLED EXCEPTION CATCHER
app.UseExceptionHandler(exceptionHandlerApp => {
    exceptionHandlerApp.Run(async context => {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/plain";

        var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        string details = exceptionFeature?.Error.Message ?? "Unknown server fault.";

        await context.Response.WriteAsync($"Error: Server crash - {details}");
    });
});

// 2. SERVE THE BUNDLED BLAZOR CLIENT FILES
//app.UseBlazorFrameworkFiles(); // Configures the server to find your published WASM binaries
app.UseStaticFiles();          // Serves indices, css, and js assets

app.MapControllers();

// 3. FALLBACK TO INDEX
// If a user refreshes an internal page route directly, serve index.html so WASM routing boots up
app.MapFallbackToFile("index.html");

 

app.MapGet("/api/csv/{year}", async (int year, IHttpClientFactory factory) =>
{
    try
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

        // REPLACE with your actual external URL
        string externalUrl = $"https://lottery.merseyworld.com/cgi-bin/lottery?days=2&Machine=Z&Ballset=0&order=0&show=1&year={year}&display=CSV";

        var csvData = await client.GetStringAsync(externalUrl);
        return Results.Text(csvData);
    }
    catch (Exception ex)
    {
        // This will show the exact error in your browser
        return Results.Text($"ERROR: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
    }
});


//        string externalUrl = $"https://lottery.merseyworld.com/cgi-bin/lottery?days=2&Machine=Z&Ballset=0&order=0&show=1&year={year}&display=CSV";


app.Run();
//---------------------end---------------------------------

/*

 */