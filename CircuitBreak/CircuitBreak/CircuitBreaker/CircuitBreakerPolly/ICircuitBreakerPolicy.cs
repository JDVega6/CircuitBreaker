
namespace CircuitBreak.CircuitBreaker.CircuitBreakerPolly
{
    public interface ICircuitBreakerPolicy
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }
}