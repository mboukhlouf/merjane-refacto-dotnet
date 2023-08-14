﻿namespace Core.Repositories
{
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }

        IProductRepository Products { get; }

        Task SaveAsync(CancellationToken cancellationToken);
    }
}
