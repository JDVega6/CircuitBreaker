using Polly.CircuitBreaker;
using Polly;
using CircuitBreak.ExceptionHandler;

namespace CircuitBreak.CircuitBreaker.CircuitBreakerPolly
{
    public class CircuitBreakerPolicy : ICircuitBreakerPolicy
    {
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public CircuitBreakerPolicy()
        {
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(20),
                    onBreak: (exception, breakDelay) =>
                    {
                        Console.WriteLine($"Open circuit. It will open for {breakDelay.TotalSeconds} seconds. (CircuitBreakerPolicy)");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Circuit in CLOSED state. The service is running. (CircuitBreakerPolicy)");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("Circuit in semi-open state.Checking service status. (CircuitBreakerPolicy)");

                    });
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(action);
        }
    }
}
