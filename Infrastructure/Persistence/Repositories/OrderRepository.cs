using Core.Repositories;
using MerjaneRefacto.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MerjaneRefacto.Infrastructure.Persistence.Repositories
{
    internal sealed class OrderRepository : IOrderRepository
    {
        private readonly DbSet<Order> orders;

        public OrderRepository(DbSet<Order> orders)
        {
            this.orders = orders;
        }

        public Order? GetOrder(long orderId)
        {
            var order = orders
                .Include(o => o.Items)
                .AsNoTracking()
                .SingleOrDefault(o => o.Id == orderId);
            return order;
        }
    }
}
