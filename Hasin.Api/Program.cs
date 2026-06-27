using Hasin.Api.Registrations;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hasin PhoneBook API",
        Version = "v1",
        Description = "PhoneBook API developed with DDD principles"
    });
});

// Dependency Injection
builder.Services
    .AddApplication()
    .AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Hasin PhoneBook API V1");

        options.RoutePrefix = string.Empty;
    });
}
app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();