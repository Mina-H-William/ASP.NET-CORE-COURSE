using MinimalAPI.Models;
using MinimalAPI.RouteGroups;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);



var app = builder.Build();

// minimal API only support json and xml in model binding of request body (not for-data, xxx-..)

app.MapGroup("/products").ProductsAPI();


app.Run();
