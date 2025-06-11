using Cassandra;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Registrar la sesión Cassandra
builder.Services.AddSingleton<Cassandra.ISession>(sp =>
{
    var cluster = Cluster.Builder()
        .AddContactPoint("127.0.0.1") // Cambia esta IP a la de tu servidor Cassandra
        .Build();

    // Conecta al keyspace 'laboratorio' (debes crearlo previamente)
    var session = cluster.Connect("laboratorio");
    return session;
});

// Registrar el servicio y la interfaz para inyección de dependencias
builder.Services.AddScoped<IPatient, PatientService>();

builder.Services.AddControllers();

// Configurar Swagger si quieres
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
