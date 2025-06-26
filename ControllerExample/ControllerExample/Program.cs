var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // add All controllers (no need for add them on by one transient)

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.Run();
