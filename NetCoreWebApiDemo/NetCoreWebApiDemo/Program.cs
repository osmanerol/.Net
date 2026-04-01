using Microsoft.EntityFrameworkCore;
using NetCoreWebApiDemo;
using NetCoreWebApiDemo.Filters;
using NetCoreWebApiDemo.Middlewares;
using NetCoreWebApiDemo.Models;
using NetCoreWebApiDemo.Profiles;
using NetCoreWebApiDemo.Repositories;
using NetCoreWebApiDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddScoped<ApiKeyAuthorizationFilter>();
builder.Services.AddScoped<ResourceLogFilter>();
builder.Services.AddScoped<ActionLogFilter>();
builder.Services.AddScoped<WrapResponseFilter>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

_ = app.Services.GetRequiredService<ConfigMonitorService>();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
