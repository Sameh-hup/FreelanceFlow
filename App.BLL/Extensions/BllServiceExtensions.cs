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
    public static IServiceCollection AddBLL(this IServiceCollection services)
    {
        // 1. Clock Service (UTC & Cairo time)
        services.AddSingleton<IClock, SystemClock>();

        // 2. Generic & Specific Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        // 3. Domain Services
        services.AddScoped<IAccessService, AccessService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IInvoiceService, InvoiceService>();

        // 4. Register FluentValidation Validators
        services.AddValidatorsFromAssembly(typeof(BllServiceExtensions).Assembly, includeInternalTypes: true);

        return services;
    }

    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        return services.AddBLL();
    }
}
