using CircuitBreak.CircuitBreaker;
using CircuitBreak.ElasticHandler;
using CircuitBreak.ExceptionHandler;
using CircuitBreak.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CircuitBreak.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PollyCircuitBreakerProductController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ICircuitBreaker _circuitBreaker;

        public PollyCircuitBreakerProductController(HttpClient httpClient, ICircuitBreaker circuitBreaker)
        {
            _httpClient = httpClient;
            _circuitBreaker = circuitBreaker;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var url = "http://localhost:9200/products/_search";

                var query = new
                {
                    query = new
                    {
                        match_all = new { }
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(query);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var searchResult = JsonConvert.DeserializeObject<ElasticSearchResponse<Product>>(responseContent);

                    var products = searchResult.hits.hits.Select(hit => hit._source);
                    return Ok(products);
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateIndex()
        {
            try
            {
                HttpResponseMessage response = null;
                HttpResponseMessage checkResponse = null;

                var indexName = "products";
                var url = $"http://localhost:9200/{indexName}";

                await _circuitBreaker.ExecuteAsync(async () =>
                {
                    checkResponse = await _httpClient.GetAsync(url);
                });

                if (checkResponse.IsSuccessStatusCode)
                {
                    return Ok("Index already exists.");
                }

                var indexConfig = new
                {
                    settings = new
                    {
                        number_of_shards = 1,
                        number_of_replicas = 1
                    },
                    mappings = new
                    {
                        properties = new
                        {
                            productId = new { type = "keyword" },
                            name = new { type = "text" },
                            description = new { type = "text" },
                            price = new { type = "float" },
                            category = new { type = "keyword" }
                        }
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(indexConfig);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                response = await _httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Index created successfully.");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (CircuitBreakerOpenException ex)
            {
                return StatusCode(503, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            try
            {
                HttpResponseMessage response = null;
                product.ProductId = Guid.NewGuid();

                var indexName = "products";
                var url = $"http://localhost:9200/{indexName}/_doc/{product.ProductId}";

                var json = System.Text.Json.JsonSerializer.Serialize(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await _circuitBreaker.ExecuteAsync(async () =>
                {
                    response = await _httpClient.PostAsync(url, content);
                });

                if (response.IsSuccessStatusCode)
                {
                    return Ok($"Product created successfully . Id: {product.ProductId}");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error: {responseContent}");
                }
            }
            catch (CircuitBreakerOpenException ex)
            {
                return StatusCode(503, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            try
            {
                HttpResponseMessage response = null;

                var indexName = "products";
                var url = $"http://localhost:9200/{indexName}/_doc/{productId}";

                await _circuitBreaker.ExecuteAsync(async () =>
                {
                    
                    response = await _httpClient.DeleteAsync(url);
                });
                

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Product deleted successfully.");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error: {responseContent}");
                }
            }
            catch (CircuitBreakerOpenException ex)
            {
                return StatusCode(503, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
