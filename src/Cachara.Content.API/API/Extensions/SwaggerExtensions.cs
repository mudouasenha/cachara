﻿using Cachara.Content.API.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cachara.Content.API.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "internal",
                new OpenApiInfo
                {
                    Title = "Cachara Content Internal API",
                    Description = "Content Internal API",
                    Version = "internal"
                }
            );

            options.SwaggerDoc(
                "public",
                new OpenApiInfo
                {
                    Title = "Cachara Content Public API", Description = "Content Public API", Version = "public"
                }
            );

            options.TagActionsBy(api =>
            {
                var tagGroups = api.CustomAttributes().OfType<TagGroup>();

                if (tagGroups.Any())
                {
                    return tagGroups.Select(x => x.Group).ToArray();
                }

                var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                if (controllerActionDescriptor != null)
                {
                    return new[] { controllerActionDescriptor.ControllerName };
                }

                throw new InvalidOperationException("Unable to determine tag for endpoint.");
            });

            options.EnableAnnotations();
            options.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string", Example = new OpenApiString("00:00:00")
            });
            options.MapType<DateOnly>(() => new OpenApiSchema
            {
                Type = "string", Format = "date", Example = new OpenApiString("2022-01-01")
            });
        });

        return services;
    }
}
