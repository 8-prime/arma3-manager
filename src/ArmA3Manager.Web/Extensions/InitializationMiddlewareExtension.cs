using System.Net;
using System.Text.Json;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Web.Models;

namespace ArmA3Manager.Web.Extensions;

public static class InitializationMiddlewareExtension
{
    public static WebApplication UseInitialization(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var metaData = context.GetEndpoint()?.Metadata.GetMetadata<RequireInitializationMetadata>();
            // No initialization required. Just call endpoint as usual
            if (metaData is null)
            {
                await next.Invoke(context);
                return;
            }

            var initManager = context.RequestServices.GetRequiredService<IInitializationInfo>();
            if (!initManager.Completed)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(initManager.InitializationResources));
                return;
            }

            await next.Invoke(context);
        });

        return app;
    }
}