﻿using MerjaneRefacto.Core.Abstractions.Services;
using MerjaneRefacto.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MerjaneRefacto.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
