using Kromi.Web.Configurations;
using Mapster;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Mapster
builder.Services.AddMapster();
// Configure Mapster
MapsterConfig.Configure();
builder.ConfigureDI();
builder.ConfigureValidations();
builder.ConfigureRepositories();
builder.ConfigureSwagger();
builder.ConfigureCompresion();
builder.ConfigureSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseAuthorization();

app.MapControllers();

app.Run();
