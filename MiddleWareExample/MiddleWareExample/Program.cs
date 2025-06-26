using MiddleWareExample.CustomMiddlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<MyMiddleware>();

var app = builder.Build();

app.Use(async (context,next) =>
{
    await context.Response.WriteAsync("Hello from MiddleWare 1\n");
    await next(context);
});

// first function return boolean when its true the second function (contains middlewares will be excuted.
app.UseWhen(
    context =>  context.Request.Query.ContainsKey("Username"),
    app =>
    {
        app.Use(async (context, next) =>
        {
            await context.Response.WriteAsync("Hello from Middleware Usewhen \n");
            await next(context);
        });
    });

// create custom middleware with interface IMiddleware
//////////////////////
//app.UseMiddleware<MyMiddleware>();

//app.UseMyCustomMiddleWare();

///////////////////////////////
/// create custom middleware with no interface (use template when create class) 
app.UseHelloCustomMiddleware();

app.Run( async context => {
    await context.Response.WriteAsync(" Hello from MiddleWare 3\n");
});
app.Run();
