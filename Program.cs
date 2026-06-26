using Microsoft.AspNetCore.Diagnostics;

;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

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

// Serves the HTML, JavaScript, and compiled WASM binaries cleanly from your copied folder
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
