using App.BLL.Interfaces;
using App.BLL.Interfaces.Services;
using App.BLL.Repositories;
using App.BLL.Services;
using App.BLL.Services.Clock;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace App.BLL.Extensions;

public static class BllServiceExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        return services.AddBLL();
    }

    public static IServiceCollection AddBLL(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();

        services.AddScoped<IAccessService, AccessService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();

        services.AddValidatorsFromAssembly(typeof(BllServiceExtensions).Assembly, includeInternalTypes: true);

        return services;
    }
}
