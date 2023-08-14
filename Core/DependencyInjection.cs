using MerjaneRefacto.Core.Abstractions.Services;
using MerjaneRefacto.Core.Services;
using MerjaneRefacto.Core.UseCases.Orders.ProcessOrder;
using Microsoft.Extensions.DependencyInjection;

namespace MerjaneRefacto.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        services.AddUseCases();

        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IProcessOrderUseCase, ProcessOrderUseCase>();

        return services;
    }
}
