using Core.Abstractions.Repositories;
using MerjaneRefacto.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MerjaneRefacto.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductRepository : IProductRepository
    {
        private readonly DbSet<Product> products;

        public ProductRepository(DbSet<Product> products)
        {
            this.products = products;
        }

        public void Update(Product product)
        {
            products.Entry(product).State = EntityState.Modified;
        }
    }
}
