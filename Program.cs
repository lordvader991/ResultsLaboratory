using Cassandra;

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
