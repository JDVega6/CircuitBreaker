namespace CircuitBreak.ElasticHandler
{
    public class ElasticSearchResponse<T>
    {
        public HitsMetadata<T> hits { get; set; }
    }

    public class HitsMetadata<T>
    {
        public IEnumerable<Hit<T>> hits { get; set; }
    }

    public class Hit<T>
    {
        public T _source { get; set; }
    }

}
