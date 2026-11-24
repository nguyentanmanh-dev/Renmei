using Microsoft.Extensions.DependencyInjection;
using Renmei.Application.Features.Kanji;

namespace Renmei.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IKanjiQueryService, KanjiQueryService>();
        return services;
    }
}