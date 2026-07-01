using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using ToyStore.ApiGateway.Extensions;
using ToyStore.ApiGateway.Persistence;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Configurations;
using ToyStore.Shipping.Worker;
using ToyStore.Shipping.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration
    .GetSection(AzureServiceBusOptions.SectionName)
    .Get<AzureServiceBusOptions>()?.ConnectionString;

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("AzureServiceBus:ConnectionString não configurada.");

builder.Services.AddSingleton(_ => new ServiceBusClient(connectionString));

builder.Services.AddDbContext<ToyStoreDbContext>(
    options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

builder.Services.AddTransient<ShippingMessageConsumer>();
builder.Services.AddHostedService<ShippingWorkerService>();

var host = builder.Build();
host.Run();
