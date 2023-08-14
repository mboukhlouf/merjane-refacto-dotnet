using Core.Abstractions.Repositories;
using MerjaneRefacto.Infrastructure.Persistence.Repositories;
using MerjaneRefacto.Presentation.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MerjaneRefacto.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                _ = options.UseInMemoryDatabase($"InMemoryDb");
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
