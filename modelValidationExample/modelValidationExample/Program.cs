using modelValidationExample.CustomModelBinders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    // insert it at 0 to override the default model binder to make it respected
    options.ModelBinderProviders.Insert(0, new PersonBinderProvider());
});

//builder.Services.AddControllers().AddXmlSerializerFormatters(); // it used when we want to accept data in xml from Request body

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
