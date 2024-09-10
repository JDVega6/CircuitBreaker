namespace CircuitBreak.ExceptionHandler
{
    public class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException()
            : base("The circuit is open. Requests cannot be processed at this time.")
        {
        }

        public CircuitBreakerOpenException(string message) : base(message)
        {
        }
    }

}
