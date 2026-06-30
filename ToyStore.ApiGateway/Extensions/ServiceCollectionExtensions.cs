using Microsoft.EntityFrameworkCore;
using ToyStore.ApiGateway.Persistence;
using ToyStore.ApiGateway.Repositories;
using ToyStore.ApiGateway.Services;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Extensions;

namespace ToyStore.ApiGateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<ToyStoreDbContext>(options =>
            options.UseInMemoryDatabase("ToyStoreDb"));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }

    public static IServiceCollection AddAppMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAzureServiceBus(configuration);
        return services;
    }
}