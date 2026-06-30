using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using ToyStore.ApiGateway.Extensions;
using ToyStore.ApiGateway.Persistence;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Configurations;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Interfaces;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Services;
using ToyStore.Payment.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);
// Lê a connection string
var connectionString = builder.Configuration
    .GetSection(AzureServiceBusOptions.SectionName)
    .Get<AzureServiceBusOptions>()?.ConnectionString;

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "A configuração 'AzureServiceBus:ConnectionString' não foi encontrada.");

// Registra o ServiceBusClient como Singleton
builder.Services.AddSingleton(_ => new ServiceBusClient(connectionString));
builder.Services.AddSingleton<IMessagePublisher, ServiceBusMessagePublisher>();

builder.Services.AddDbContext<ToyStoreDbContext>(options => options.UseInMemoryDatabase("ToyStoreDb"));
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

// Registra o consumidor e o Worker
builder.Services.AddTransient<PaymentProcessor>();
builder.Services.AddTransient<PaymentMessageConsumer>();
builder.Services.AddHostedService<PaymentWorkerService>();

var host = builder.Build();
host.Run();
