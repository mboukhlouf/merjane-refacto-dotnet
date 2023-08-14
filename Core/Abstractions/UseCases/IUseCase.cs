namespace MerjaneRefacto.Core.Abstractions.UseCases
{
    public interface IUseCase<TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request);
    }
}
