using CRUDExample;
using CRUDExample.Middlewares;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

#region Logging configuration fro built in logging

// Old way of configuring logging
//builder.Host.ConfigureLogging(loggingProvider =>
//{
//    loggingProvider.ClearProviders();
//    loggingProvider.AddConsole();
//    loggingProvider.AddDebug();
//});

// New way of configuring logging using the builder
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();

#endregion

//Serilog configure
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration) // read configuration settings from built-in Iconfiguration
    .ReadFrom.Services(services); // read out current app's services and make them available to serilog
});

// call extension method to configure services
builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

//builder.Services.AddHttpContextAccessor(); // Add HttpContextAccessor to DI Container to access HttpContext
// outside web project (in class libirary like services)

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // we can use UseStatusCodePagesWithRedirects to handle specific status codes like 500, 404, etc.
    // app.UseStatusCodePagesWithRedirects("/StatusCodes/{0}"); // Redirects to Error action with status code {0}
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware(); // Add custom exception handling middleware
}

if (app.Environment.IsProduction())
{
    app.UseHsts(); // Use HTTP Strict Transport Security (HSTS) in production, enforce all requests to use HTTPS
}
app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS


app.UseSerilogRequestLogging(); // Add Serilog request logging middleware

app.UseHttpLogging();

app.Logger.LogInformation($"Current enviroment: {app.Environment.EnvironmentName}");

if (!app.Environment.IsEnvironment("Test"))
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add authentication middleware to the pipeline (Reading identity Cookie)
app.UseAuthorization(); // Add authorization middleware to the pipeline (Checking if user is authorized)

// Conventional Routing using Top level statements to configure endpoints
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Persons}/{action=Index}/{id?}"
//);

// Conventional Routing For Areas
//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{area:exists}/{controller=Persons}/{action=Index}/{id?}" // :exists to make sure the area is real area.
//);

app.MapControllers();

app.Run();


// partail keyword is used to split the Program class into multiple files and compiler will combine them at compile time as 
// a single class. This is useful for organizing large classes or when you want to separate the main entry point from other logic.
public partial class Program { }