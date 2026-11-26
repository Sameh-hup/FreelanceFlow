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
        // 1. Clock Service (UTC)
        services.AddSingleton<IClock, SystemClock>();

        // 2. Generic Repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // 3. Concrete Domain Repositories
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        // 4. Domain & Application Services
        services.AddScoped<IAccessService, AccessService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IAuthService, AuthService>();

        // 5. Register FluentValidation Validators
        services.AddValidatorsFromAssembly(typeof(BllServiceExtensions).Assembly, includeInternalTypes: true);

        return services;
    }
}
