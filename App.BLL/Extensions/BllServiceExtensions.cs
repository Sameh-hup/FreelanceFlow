using App.BLL.Interfaces;
using App.BLL.Repositories;
using App.BLL.Services.Clock;
using Microsoft.Extensions.DependencyInjection;

namespace App.BLL.Extensions;

public static class BllServiceExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        // 1. Clock Service (UTC)
        services.AddSingleton<IClock, SystemClock>();

        // 2. Generic Repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Note: Specific repositories and services can be registered here as implemented

        return services;
    }
}
