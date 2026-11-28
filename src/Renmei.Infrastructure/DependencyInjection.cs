using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Renmei.Application.Abstractions;
using Renmei.Infrastructure.Persistence;
using Renmei.Infrastructure.Security;
using Renmei.Infrastructure.Seeding;
using Renmei.Infrastructure.Time;
using Renmei.Infrastructure.Auth;
using Renmei.Application.Features.Auth;

namespace Renmei.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddOptions<AdminSeedOptions>().Bind(configuration.GetSection(AdminSeedOptions.Section));
        services.Configure<PasswordHasherOptions>(options =>
        {
            options.IterationCount = configuration.GetValue<int?>("Security:PasswordHashing:IterationCount") ?? 210_000;
        });

        services.AddSingleton<Application.Abstractions.IPasswordHasher, IdentityPasswordHasher>();
        services.AddScoped<IDataSeeder, AdminSeeder>();
        services.AddSingleton<IClock, SystemClock>();

        services.AddOptions<Renmei.Application.Features.Auth.TokenOptions>().Bind(configuration.GetSection("Auth:Tokens"));
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .Validate(
                o => !string.IsNullOrWhiteSpace(o.Issuer)
                     && !string.IsNullOrWhiteSpace(o.Audience)
                     && TryDecodeBase64Key(o.SigningKey, out var bytes)
                     && bytes.Length >= 32,
                "Jwt:Issuer, Jwt:Audience, and a Base64 Jwt:SigningKey of at least 32 bytes are required.")
            .ValidateOnStart();

        services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();
        services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();

        static bool TryDecodeBase64Key(string? raw, out byte[] bytes)
        {
            bytes = [];
            if (string.IsNullOrWhiteSpace(raw)) return false;
            try { bytes = Convert.FromBase64String(raw); return true; }
            catch (FormatException) { return false; }
        }

        return services;
    }
}