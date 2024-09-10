
namespace CircuitBreak.CircuitBreaker
{
    public interface ICircuitBreaker
    {
        Task ExecuteAsync(Func<Task> action);
    }
}