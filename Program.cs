using OrderService.Models;
using OrderService.Repositories;
using ISession = Cassandra.ISession;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Conexión a Cassandra
builder.Services.AddSingleton<ISession>(provider =>
{
    return OrderService.Cassandra.CassandraConnector.Connect(builder.Configuration);
});
builder.Services.AddSingleton<CassandraOrderRepository>();

// Agregar controladores
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Service API", Version = "v1" });
});

var app = builder.Build();

// Middleware Swagger en cualquier entorno
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API v1");
    c.RoutePrefix = string.Empty; // Hace que Swagger se cargue en la raíz "/"
});

app.UseHttpsRedirection();

// Mapeo de rutas de controladores
app.MapControllers();

app.Run();
