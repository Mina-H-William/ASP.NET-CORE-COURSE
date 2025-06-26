var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var countries = new Dictionary<int, string>
{
    {1,"united states"},
    {2,"canda"},
    {3,"united Kingdom"},
    {4,"india"},
}; 

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.Map("/countries", async context =>
    {
        foreach(var country in countries)
        {
        await context.Response.WriteAsync(country.Value + "\n");
        }
    });


    endpoints.Map("/countries/{id:int}", async context =>
    {
        int id = Convert.ToInt32(context.Request.RouteValues["id"]);
        if (id >= 1 && id <= 5)
        {
            await context.Response.WriteAsync($"name of the country {id} is {countries[id]}");
        }
        else if (id > 5)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync($"No country");
        }
        else
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync($"Country id should be between 1 and 100");
        }
    });
});

app.Run(async context =>
{
    await context.Response.WriteAsync($"this url: {context.Request.Path} is not found");
});

app.Run();