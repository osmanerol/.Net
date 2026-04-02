using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NetCoreWebApiDemo;
using NetCoreWebApiDemo.Filters;
using NetCoreWebApiDemo.Middlewares;
using NetCoreWebApiDemo.Models;
using NetCoreWebApiDemo.Profiles;
using NetCoreWebApiDemo.Repositories;
using NetCoreWebApiDemo.Services;
using Serilog;
using System.Reflection;

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

//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<GlobalExceptionFilter>();
//});
builder.Services.AddControllers();
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
});
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
/*
builder.Services.AddScoped<ApiKeyAuthorizationFilter>();
builder.Services.AddScoped<ResourceLogFilter>();
builder.Services.AddScoped<ActionLogFilter>();
builder.Services.AddScoped<WrapResponseFilter>();
*/

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

_ = app.Services.GetRequiredService<ConfigMonitorService>();

app.UseHttpsRedirection();

app.UseCorrelationId();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
