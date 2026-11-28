using Microsoft.Extensions.DependencyInjection;
using Renmei.Application.Features.Kanji;
using Renmei.Application.Features.KanjiAdmin;
using Renmei.Application.Features.Auth;

namespace Renmei.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IKanjiQueryService, KanjiQueryService>();
        services.AddScoped<IAdminKanjiService, AdminKanjiService>();
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}