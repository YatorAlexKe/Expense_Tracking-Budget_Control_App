using System.Text;
using FinanceTracker.API.Middleware;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.Mappings;
using FinanceTracker.Application.Services;
using FinanceTracker.Infrastructure.Data;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FinanceTracker.API;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Database - SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=financetracker.db"));

// AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

// Repositories
builder.Services.AddScoped<IUserRepository,     UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IExpenseRepository,  ExpenseRepository>();
builder.Services.AddScoped<IBudgetRepository,   BudgetRepository>();
builder.Services.AddScoped<ICryptoRepository,   CryptoRepository>();

// Services
builder.Services.AddScoped<IAuthService,      AuthService>();
builder.Services.AddScoped<ICategoryService,  CategoryService>();
builder.Services.AddScoped<IExpenseService,   ExpenseService>();
builder.Services.AddScoped<IBudgetService,    BudgetService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICryptoService,    CryptoService>();
// Mock price service — swap for a real HTTP client in production
builder.Services.AddHttpClient<ICryptoPriceService, CoinGeckoPriceService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Add("User-Agent", "FinanceTracker/1.0");
});
builder.Services.AddScoped<IEmailService, EmailService>();
// Monthly report background job
builder.Services.AddHostedService<MonthlyReportJob>();


// JWT
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwt["Issuer"],
        ValidAudience            = jwt["Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(key)
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinanceTracker API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {{
        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }},
        Array.Empty<string>()
    }});
});

var app = builder.Build();


app.UseMiddleware<ExceptionMiddleware>();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinanceTracker API v1"); c.RoutePrefix = string.Empty; });
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"\n\n STARTUP ERROR: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Console.ReadLine();
}