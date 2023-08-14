using Core.Repositories;
using MerjaneRefacto.Core.Entities;
using MerjaneRefacto.Presentation.Database.Context;

namespace MerjaneRefacto.Infrastructure.Persistence.Repositories
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;
        private readonly OrderRepository orderRepository;

        public UnitOfWork(AppDbContext dbContext)
        {
            this.dbContext = dbContext;

            orderRepository = new OrderRepository(dbContext.Set<Order>());
        }

        public IOrderRepository Orders => orderRepository;

        public IProductRepository Products => throw new NotImplementedException();

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
