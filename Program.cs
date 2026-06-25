//this version avoids CORS and the Release version output folder should be copy pasted here in this project folder

 
using System.Net.Http;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

var app = builder.Build();
//app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapGet("/api/csv/{year}", async (int year) =>
{
    try
    {
        using var client = new HttpClient();
        var url = $"https://lottery.merseyworld.com/cgi-bin/lottery?days=2&Machine=Z&Ballset=0&order=0&show=1&year={year}&display=CSV";
        var csvData = await client.GetStringAsync(url);
        return Results.Text(csvData);
    }
    catch (Exception ex)
    {
        // Return error message clearly marked with "error:" prefix
        return Results.Text( $"Error: {ex.Message}");
    }
});




//app.MapGet("/", () => Results.Text("...API Loaded And Waiting..."));//dummy page better than error page
app.MapFallbackToFile("index.html");


app.Run();