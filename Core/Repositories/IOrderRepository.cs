using MerjaneRefacto.Core.Entities;

namespace Core.Repositories
{
    public interface IOrderRepository
    {
        Order? GetOrder(long orderId); 
    }
}
