namespace CircuitBreak.CircuitBreaker.CircuitBreakerManual
{
    public interface ICircuitBreakerManual
    {
        Task ExecuteAsync(Func<Task> action);
    }
}