using Azure.Messaging.ServiceBus;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Configurations;
using ToyStore.Inventory.Worker;
using ToyStore.Inventory.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration
    .GetSection(AzureServiceBusOptions.SectionName)
    .Get<AzureServiceBusOptions>()?.ConnectionString;

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("AzureServiceBus:ConnectionString não configurada.");

builder.Services.AddSingleton(_ => new ServiceBusClient(connectionString));
builder.Services.AddTransient<InventoryMessageConsumer>();
builder.Services.AddHostedService<InventoryWorkerService>();

var host = builder.Build();
host.Run();
