using ConfigurationExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//supply an object of weatherapioptions (with "weatherapi" section) as a service 
builder.Services.Configure<WeatherApiOptions>(builder.Configuration.GetSection("weatherapi"));



//builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
//{
//    config.AddJsonFile("MyOwnConfig.json", optional: true, reloadOnChange: true);
//});

builder.Configuration.AddJsonFile("MyOwnConfig.json", optional: true, reloadOnChange: true);

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();


//app.UseEndpoints(endpoints =>
//{
//    endpoints.Map("/", async (context) =>
//    {
//        await context.Response.WriteAsync(app.Configuration["MyKey"] + "\n");

//        // if not specify <Type> in getvalue it will return object, so should write the type you want

//        await context.Response.WriteAsync(app.Configuration.GetValue<String>("MyKey") + "\n");

//        await context.Response.WriteAsync(app.Configuration.GetValue<int>("x",10) + "\n");

//    });
//});

app.MapControllers();


app.Run();
