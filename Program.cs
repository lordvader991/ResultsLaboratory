using TestTypeService.Data;
using TestTypeService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CassandraConnector>();
builder.Services.AddSingleton(provider => provider.GetRequiredService<CassandraConnector>().Connect());
builder.Services.AddSingleton<TestTypeService.Services.TestTypeService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
//algo
