using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using Repositories;
using Services;

using System.Security.Cryptography;
using System.Text;
using WebApiShop.MiddleWare;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserServices, UserServices>();  
builder.Services.AddScoped<IPasswordServices, PasswordServices>();
builder.Services.AddScoped<IProductsServices, ProductsServices>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<ICategoriesServices, CategoriesServices>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository> ();
builder.Services.AddScoped<IOrdersServices, OrdersServices>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<OrderStatusUpdaterService>();
builder.Services.AddHostedService<EmbeddingIndexService>();
builder.Services.AddHttpClient();
// Every call to the Python AI service goes through this address, set in appsettings.json
var aiServiceUrl = new Uri(builder.Configuration["AiService:BaseUrl"]!);
builder.Services.AddHttpClient("AiService", c => c.BaseAddress = aiServiceUrl);
builder.Services.AddHttpClient<IVacationService, VacationService>(c => c.BaseAddress = aiServiceUrl);

builder.Host.UseNLog();

builder.Services.AddDbContext<ApiShopContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("ApiShop")));
// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });


builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigins",
    policy =>
    {
        policy.SetIsOriginAllowed(origin =>
              {
                  if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                      return false;

                  return uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase);
              })
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// The signing key must not live in the repository: set Jwt:Key with
// `dotnet user-secrets` locally, or the Jwt__Key environment variable on a server.
string jwtKey = builder.Configuration["Jwt:Key"] ?? "";
if (jwtKey.StartsWith("REPLACE") || jwtKey.Length < 32)
{
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException("Jwt:Key is missing or a placeholder.");
    // Development only: a random key per run, so the app starts without setup
    jwtKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    builder.Configuration["Jwt:Key"] = jwtKey;
}
var key = Encoding.ASCII.GetBytes(jwtKey);

// ����� ������ ������
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // ���� ���� �� ������
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    // Every endpoint needs a signed-in user unless it says [AllowAnonymous]
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

var app = builder.Build();

app.UseCors("AllowAngularOrigins");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseErrorHandling();

app.UseRating();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


