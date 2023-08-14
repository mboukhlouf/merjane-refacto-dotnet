using MerjaneRefacto.Core.Entities;

namespace MerjaneRefacto.Core.Abstractions.Repositories;

public interface IProductRepository
{
    void Update(Product product);
}
