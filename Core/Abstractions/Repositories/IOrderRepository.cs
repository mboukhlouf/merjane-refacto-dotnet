using MerjaneRefacto.Core.Entities;

namespace Core.Abstractions.Repositories;

public interface IOrderRepository
{
    Order? GetOrder(long orderId);
}
