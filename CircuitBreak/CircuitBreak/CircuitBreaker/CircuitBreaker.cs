using CircuitBreak.ExceptionHandler;

namespace CircuitBreak.CircuitBreaker
{
    public class CircuitBreaker : ICircuitBreaker
    {
        private enum CircuitState
        {
            Closed,
            Open,
            HalfOpen
        }

        private int failureCount = 0;
        private readonly int failureThreshold = 3;
        private TimeSpan openToHalfOpenWaitTime = TimeSpan.FromSeconds(15);
        private DateTime lastStateChangeTime;
        private CircuitState state = CircuitState.Closed;

        public async Task ExecuteAsync(Func<Task> action)
        {
            switch (state)
            {
                case CircuitState.Closed:
                    await ExecuteActionAsync(action);
                    break;

                case CircuitState.Open:
                    if (DateTime.UtcNow - lastStateChangeTime >= openToHalfOpenWaitTime)
                    {
                        state = CircuitState.HalfOpen;
                        Console.WriteLine("Circuit in semi-open state.");
                        await ExecuteActionAsync(action);
                    }
                    else
                    {
                        throw new CircuitBreakerOpenException("Circuit is open. Please try again later.");
                    }
                    break;

                case CircuitState.HalfOpen:
                    await ExecuteActionAsync(action);
                    break;
            }
        }

        private async Task ExecuteActionAsync(Func<Task> action)
        {
            try
            {
                await action();

                if (state == CircuitState.HalfOpen)
                {
                    Console.WriteLine("Circuit closed again after successful test.");
                    state = CircuitState.Closed;
                    failureCount = 0;
                }
            }
            catch (Exception)
            {
                failureCount++;

                if (failureCount >= failureThreshold)
                {
                    state = CircuitState.Open;
                    lastStateChangeTime = DateTime.UtcNow;
                    Console.WriteLine($"Open circuit. It will open for {openToHalfOpenWaitTime.TotalSeconds} seconds.");

                    //openToHalfOpenWaitTime *= 2;
                }
                else if (state == CircuitState.HalfOpen)
                {
                    state = CircuitState.Open;
                    lastStateChangeTime = DateTime.UtcNow;
                    Console.WriteLine($"Circuit reopened after a fault in half-open state. It will open for {openToHalfOpenWaitTime.TotalSeconds} seconds.");
                }

                throw;
            }
        }

    }
}
