using MinimalAPI.EndpointFilters;
using MinimalAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace MinimalAPI.RouteGroups
{
    public static class Productsgroup
    {
        private static List<Product> products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1" },
            new Product { Id = 2, Name = "Product 2" }
        };

        public static RouteGroupBuilder ProductsAPI(this RouteGroupBuilder group)
        {

            group.MapGet("/", (HttpContext context) =>
            {
                //var content = string.Join("\n", products.Select(tmp => tmp.ToString()));

                //await context.Response.WriteAsync(JsonSerializer.Serialize(products));

                return Results.Ok(products);
            });

            group.MapGet("/{id:int}", (HttpContext context, int id) =>
            {
                Product? product = products.FirstOrDefault(tmp => tmp.Id == id);

                if (product == null)
                {
                    //context.Response.StatusCode = 404; // Not Found
                    //await context.Response.WriteAsync("Product not found");

                    return Results.NotFound(new { message = "Product not found" });
                }

                //await context.Response.WriteAsync(JsonSerializer.Serialize(product));

                return Results.Ok(product);
            });

            group.MapPost("/", (HttpContext context, Product product) =>
            {
                products.Add(product);
                //await context.Response.WriteAsync("POST - Product Added successfully");

                return Results.Ok(new { message = "POST - Product Added successfully" });
            })
                .AddEndpointFilter<CustomEndpointFilter>()
                .AddEndpointFilter(async (EndpointFilterInvocationContext context, EndpointFilterDelegate next) =>
                 {
                     // before logic
                     var product = context.Arguments.OfType<Product>().FirstOrDefault();
                     if (product == null)
                     {
                         return Results.BadRequest(new { message = "Product is required" });
                     }

                     var validationContext = new ValidationContext(product);
                     var validationResults = new List<ValidationResult>();
                     bool isValid = Validator.TryValidateObject(product, validationContext, validationResults, true);

                     if (!isValid)
                     {
                         return Results.BadRequest(validationResults.FirstOrDefault()?.ErrorMessage);
                     }

                     var result = await next(context);

                     // after logic 

                     return result;
                 });

            group.MapPut("/{id}", (HttpContext context, int id, Product product) =>
            {
                Product? currentProduct = products.FirstOrDefault(tmp => tmp.Id == id);

                if (currentProduct == null || id != product.Id)
                {
                    //context.Response.StatusCode = 400; // bad request
                    //await context.Response.WriteAsync("Invalid id");

                    return Results.ValidationProblem(new Dictionary<string, string[]> {
                        { "id", new string[] { "Invalid id" } }
                    });
                }

                currentProduct.Name = product.Name;

                //await context.Response.WriteAsync("PUT - Product Updated successfully");

                return Results.Ok(new { message = "PUT - Product Updated successfully" });
            });

            group.MapDelete("/{id}", (HttpContext context, int id) =>
            {
                Product? currentProduct = products.FirstOrDefault(tmp => tmp.Id == id);

                if (currentProduct == null)
                {
                    //context.Response.StatusCode = 400; // bad request
                    //await context.Response.WriteAsync("Invalid id");

                    return Results.ValidationProblem(new Dictionary<string, string[]> {
                        { "id", new string[] { "Invalid id" } }
                    });
                }

                products.Remove(currentProduct);

                //await context.Response.WriteAsync("Delete - Deleted Successfully");

                return Results.Ok(new { message = "Delete - Deleted Successfully" });
            });

            return group;
        }
    }
}
