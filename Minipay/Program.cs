using Minipay.Api.Endpoints;
using Minipay.Application;
using Minipay.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddService(
    builder.Configuration.GetConnectionString("Minipay")
    ?? throw new InvalidOperationException(
        "Connection string 'Minipay' not found in appsettings.json"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPaymentEndpoints();

app.Run();