using Kromi.Api.Configurations;
using Kromi.Api.Middlewares;
using Kromi.Application;
using Kromi.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
string corsName = "AllowSpecificOrigin";
// Add services to the container.
builder.ConfigureCors(corsName);
builder.Services.AddControllers();
builder.ConfigureSecurity();
builder.ConfigureControlador();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.ConfigureSwagger();
builder.ConfigureSerilog();
builder.Services.AddResponseCompression();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    options.DefaultModelsExpandDepth(-1);
    options.DisplayOperationId();
    options.DisplayRequestDuration();
    options.EnableValidator();
    options.EnablePersistAuthorization();
});
app.UseRouting();
app.UseCors(corsName);
app.UseMiddleware(typeof(SecureDownloadUrlsMiddleware));
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseResponseCompression();
app.ConfigureExceptionHandler(builder!);
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Files", "Images")),
    RequestPath = new PathString("/Files/Images")
});

await app.Migrar();
await app.RunAsync();
