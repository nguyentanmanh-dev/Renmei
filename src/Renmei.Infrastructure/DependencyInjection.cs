using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Renmei.Application.Abstractions;
using Renmei.Infrastructure.Persistence;
using Renmei.Infrastructure.Security;
using Renmei.Infrastructure.Seeding;
using Renmei.Infrastructure.Time;

namespace Renmei.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddOptions<AdminSeedOptions>().Bind(configuration.GetSection(AdminSeedOptions.Section));
        services.AddSingleton<Application.Abstractions.IPasswordHasher, IdentityPasswordHasher>();
        services.AddScoped<IDataSeeder, AdminSeeder>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}