using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Services;
public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IServiceManager, ServiceManager>();

        services.Configure<JWTOptions>(configuration.GetSection("JWTOptions"));



        services.AddAutoMapper(typeof(Services.AssemblyReference).Assembly);
        return services;
    }
}
