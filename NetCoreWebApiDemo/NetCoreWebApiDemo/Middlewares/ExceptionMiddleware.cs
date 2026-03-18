using Microsoft.AspNetCore.Mvc;
using NetCoreWebApiDemo.Exceptions;
using System.Net;
using System.Text.Json;

namespace NetCoreWebApiDemo.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occured! TraceId {TraceId}", context.TraceIdentifier);
                var problem = CreateProblemDetails(ex, context);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
            }
        }

        private ProblemDetails CreateProblemDetails(Exception ex, HttpContext context) 
        {
            var problem = new ProblemDetails
            {
                Instance = context.Request.Path,
                Detail = _env.IsDevelopment() ? ex.ToString() : "Server error occured.",
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Server Error"
            };
            problem.Extensions["TraceId"] = context.TraceIdentifier;
            if (ex is NotFoundException)
            {
                problem.Status = (int)HttpStatusCode.NotFound;
                problem.Title = "Resource not found.";
            }
            return problem;
        }
    }
}
