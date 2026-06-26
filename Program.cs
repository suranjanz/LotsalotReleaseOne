using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddControllers();

var app = builder.Build();

// GLOBAL UNHANDLED EXCEPTION CATCHER
app.UseExceptionHandler(exceptionHandlerApp => {
    exceptionHandlerApp.Run(async context => {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/plain";
        
        var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        string details = exceptionFeature?.Error.Message ?? "Unknown server fault.";
        
        await context.Response.WriteAsync($"Error: Server crash - {details}");
    });
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

//app.MapGet("/api/csv/{year}", async (int year, IHttpClientFactory factory) =>
//{
//    try
//    {
//        var client = factory.CreateClient();
//        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
//        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

//        // REPLACE with your actual external URL
//        string externalUrl  = $"https://lottery.merseyworld.com/cgi-bin/lottery?days=2&Machine=Z&Ballset=0&order=0&show=1&year={year}&display=CSV";

//        var csvData = await client.GetStringAsync(externalUrl);
//        return Results.Text(csvData);
//    }
//    catch (Exception ex)
//    {
//        // This will show the exact error in your browser
//        return Results.Text($"Error : {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
//    }
//});

app.MapFallbackToFile("index.html");

app.Run();