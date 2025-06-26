using AssisgnmentMiddleware.CustomMiddleware;
using Microsoft.AspNetCore.WebUtilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<CheckPathMiddleware>();

var app = builder.Build();


app.UseCheckPathMiddleware();

app.Use(async (context, next) =>
{
    StreamReader SReader = new StreamReader(context.Request.Body);
    string rawQuery = await SReader.ReadToEndAsync();
    var parsedQuery = QueryHelpers.ParseQuery(rawQuery);
    if(parsedQuery != null && parsedQuery.ContainsKey("email") && parsedQuery.ContainsKey("password"))
    {
        var email = parsedQuery["email"];
        var password = parsedQuery["password"];
        if(email == "admin@example.com" && password == "admin1234") await context.Response.WriteAsync("successful login");
        else await context.Response.WriteAsync("invalid login");
    }
    else if (parsedQuery == null)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Invalid input for email \n");
        await context.Response.WriteAsync("Invalid input for password \n");
    }
    else
    {
        context.Response.StatusCode = 400;
        if (!parsedQuery.ContainsKey("email"))
        {
            await context.Response.WriteAsync("Invalid input for email \n");
        }
        if (!parsedQuery.ContainsKey("password"))
        {
            await context.Response.WriteAsync("Invalid input for password \n");
        }
    }
    await next(context);
});

app.Run();
