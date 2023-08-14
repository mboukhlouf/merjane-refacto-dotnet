using MerjaneRefacto.Core.Abstractions.Services;

namespace MerjaneRefacto.Infrastructure.Services
{
    internal sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;

        public DateTime Today => DateTime.Today;
    }
}
