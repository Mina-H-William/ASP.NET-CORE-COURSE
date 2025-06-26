using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Old Way to add services
//builder.Services.Add(new ServiceDescriptor
//(
//    typeof(ICitiesService),
//    typeof(CitiesService),
//    ServiceLifetime.Transient
//));

//New Way (Best) 
//builder.Services.AddTransient<ICitiesService, CitiesService>();

builder.Services.AddScoped<ICitiesService, CitiesService>();

//transient: per injection
//Scoped: for every Scope (Request) 
//Signleton: for entire application lifetime 

// in singleton if i store data in list or Dictionary you must use a concurrent one to make it right for threads

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.Run();
