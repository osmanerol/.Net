using CorrelationId;
using CorrelationId.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using NetCoreWebApiDemo;
using NetCoreWebApiDemo.Authorization.Handler;
using NetCoreWebApiDemo.Authorization.Requirement;
using NetCoreWebApiDemo.Filters;
using NetCoreWebApiDemo.HealthChecks;
using NetCoreWebApiDemo.Middlewares;
using NetCoreWebApiDemo.Models;
using NetCoreWebApiDemo.Profiles;
using NetCoreWebApiDemo.Repositories;
using NetCoreWebApiDemo.Services;
using Serilog;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} [CID:{CorrelationId}] {Level:u3} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/app-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Product", policy =>
    {
        policy.RequireClaim("product", "true");
    });
    options.AddPolicy("AdminProduct", policy =>
    {
        policy.RequireRole("Admin");
        policy.RequireClaim("product", "true");
    });
    options.AddPolicy("SameCompanyPolicy", policy =>
    {
        policy.Requirements.Add(new SameCompanyRequirement());
    });
});
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<GlobalExceptionFilter>();
//});
builder.Services.AddControllers();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<IAuthorizationHandler, SameCompanyHandler>();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var env = builder.Environment;
Console.WriteLine(env.EnvironmentName);
builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Format : Bearer {token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please retry later.", token);
    };
    /*
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueLimit = 0;
    });
    */
    /*
    options.AddPolicy("user-sliding", context =>
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: userId,
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4,
                QueueLimit = 0
            });
    });
    */
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: "global",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
});
/*
builder.Services.AddScoped<ApiKeyAuthorizationFilter>();
builder.Services.AddScoped<ResourceLogFilter>();
builder.Services.AddScoped<ActionLogFilter>();
builder.Services.AddScoped<WrapResponseFilter>();
*/

builder.Services.AddMemoryCache();

var config = builder.Configuration;
string connection = config.GetConnectionString("DefaultConnection") ?? "";
string applicationName = config["AppSettings:ApplicationName"] ?? "";
string version = config["AppSettings:Version"] ?? "";

Console.WriteLine(applicationName);
Console.WriteLine(version);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connection)
); 
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IConfigCompareService, ConfigCompareService>();
builder.Services.AddSingleton<ConfigMonitorService>();

builder.Services.AddDefaultCorrelationId(options =>
{
    options.AddToLoggingScope = true;
});

builder.Services.AddHealthChecks()
    .AddSqlServer(
    name: "sqlserver",
    connectionString: connection,
    failureStatus: HealthStatus.Unhealthy,
    timeout: TimeSpan.FromSeconds(3))
    .AddCheck<CriticalConfigHealthCheck>("critical_config");

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

_ = app.Services.GetRequiredService<ConfigMonitorService>();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseCorrelationId();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
