using OrderService.Models;
using OrderService.Repositories;
using ISession = Cassandra.ISession;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Security.Claims;

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
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT key is not configured. Please set 'Jwt:Key' in your configuration.");
        }
        var key = Encoding.UTF8.GetBytes(jwtKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            NameClaimType = ClaimTypes.Email,
        };
    });

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
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
