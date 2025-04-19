using FluentResults;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Kromi.Api.Middlewares
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, WebApplicationBuilder builder)
        {
            var logger = builder?.Logging?.Services?.BuildServiceProvider()?.GetRequiredService<ILogger<Program>>()!;
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (context.Features != null)
                    {
                        logger.LogError(contextFeature?.Error, "Exception en la aplicacion");
                        string json = JsonSerializer.Serialize(Result.Fail("Error no controlado en la aplicacion, contacte con el administrador").Errors);
                        await context.Response.WriteAsync(json);
                    }
                });
            });
        }
    }
}
