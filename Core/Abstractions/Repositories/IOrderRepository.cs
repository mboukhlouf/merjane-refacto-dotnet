using MerjaneRefacto.Core.Entities;

namespace MerjaneRefacto.Core.Abstractions.Repositories;

public interface IOrderRepository
{
    Order? GetOrder(long orderId);
}
