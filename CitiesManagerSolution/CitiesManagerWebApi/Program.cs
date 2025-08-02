using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.Services;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json")); // for all action they return content type application/json
    options.Filters.Add(new ConsumesAttribute("application/json")); // for all action they accept content type application/json

    // can be added in Authorization serivce as fallback policy
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy: policy));

}).AddXmlSerializerFormatters(); // to support XML serialization in addition to JSON serialization

builder.Services.AddApiVersioning(config =>
{
    config.ApiVersionReader = new UrlSegmentApiVersionReader(); // read version from URL segment, e.g. /api/v2/cities

    //config.ApiVersionReader = new HeaderApiVersionReader(); // read version from HTTP header, e.g. api-version: 2.0

    // read version from query string, e.g. /api/cities?api-version=1.0
    //default value is api-version but in the below example we change it to version
    //config.ApiVersionReader = new QueryStringApiVersionReader("version");

    config.DefaultApiVersion = new ApiVersion(1, 0); // set default API version to 1.0 if not specified in the URL or header
    config.AssumeDefaultVersionWhenUnspecified = true; // assume default version when not specified
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Swagger
builder.Services.AddEndpointsApiExplorer(); // geenrates description for API endpoints (help to read endpoints)
// generates OpenAPI specification for the API
builder.Services.AddSwaggerGen(options =>
{
    //AppContext.BaseDirectory: is the directory where the application is running
    //(EX: CitiesManagerWebApi\bin\Debug\net8.0)
    //Environment.CurrentDirectory: is the current working directory of the application(EX: \CitiesManagerWebApi)

    // to use IncludeXmlComments you should add Documentation file in the project properties (Checkbox in ouptut)
    //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));

    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "cities Web API", Version = "1,0" });
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "cities Web API", Version = "2,0" });
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true; // substitute version in URL, e.g. /api/v1/cities
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!)
                     .WithHeaders("Authorization", "origin", "accept", "content-type")
                     .AllowAnyMethod()
                     .AllowCredentials(); // allow credentials (cookies, authorization headers, etc.)
    });

    options.AddPolicy("4100Client", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4100")
                     .WithHeaders("Authorization", "origin", "accept")
                     .WithMethods("GET");
    });
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
.AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

//JWT Authentication

builder.Services.AddTransient<IJwtService, JwtService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // default scheme for authentication (try first)

    // default scheme for challenge (if authentication fails) (Used when there is two ways to authanticate)
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = true, // validate the audience of the token
        ValidAudience = builder.Configuration["Jwt:Audience"], // valid audience for the token

        ValidateIssuer = true, // validate the issuer of the token
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // valid issuer for the token

        ValidateLifetime = true, // validate the lifetime of the token

        ValidateIssuerSigningKey = true, // validate the signing key of the token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSwagger();   //Create endpoint for swagger.json
// creates UI for testing API endpoints
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");
});

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
