using Core.Abstractions.Repositories;
using MerjaneRefacto.Core.Entities;
using MerjaneRefacto.Presentation.Database.Context;

namespace MerjaneRefacto.Infrastructure.Persistence.Repositories;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext dbContext;
    private readonly OrderRepository orderRepository;
    private readonly ProductRepository productRepository;

    public UnitOfWork(AppDbContext dbContext)
    {
        this.dbContext = dbContext;

        orderRepository = new OrderRepository(dbContext.Set<Order>());
        productRepository = new ProductRepository(dbContext.Set<Product>());
    }

    public IOrderRepository Orders => orderRepository;

    public IProductRepository Products => productRepository;

    public Task SaveAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
