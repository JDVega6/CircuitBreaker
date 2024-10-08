using CircuitBreak.CircuitBreaker.CircuitBreakerManual;
using CircuitBreak.CircuitBreaker.CircuitBreakerPolly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<ICircuitBreakerManual, CircuitBreakerManual>();
builder.Services.AddSingleton<ICircuitBreakerPolicy, CircuitBreakerPolicy>();
builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
