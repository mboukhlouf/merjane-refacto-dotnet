namespace MerjaneRefacto.Core.Abstractions.Services
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }

        DateTime Today { get; }
    }
}
