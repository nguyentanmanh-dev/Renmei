using Microsoft.Extensions.DependencyInjection;
using Renmei.Application.Features.Kanji;
using Renmei.Application.Features.KanjiAdmin;

namespace Renmei.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IKanjiQueryService, KanjiQueryService>();
        services.AddScoped<IAdminKanjiService, AdminKanjiService>();
        return services;
    }
}