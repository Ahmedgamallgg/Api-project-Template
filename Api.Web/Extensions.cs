using Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Authentication;
using Shared.ErrorModels;
using System.Text;

namespace Api.Web;

public static class Extensions
{
    public async static Task<WebApplication> InitializeDataBaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await dbInitializer.InitializeAsync();
        await dbInitializer.InitializeIdentityAsync();
        return app;
    }


    public static IServiceCollection AddWebApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = (context) =>
            {
                // Get the Invalid Model State Entries 
                var errors = context.ModelState.Where(m => m.Value.Errors.Any())
                .Select(m => new ValidationError
                {
                    Field = m.Key,
                    Errors = m.Value.Errors.Select(error => error.ErrorMessage)
                });

                var response = new ValidationErrorResponse { ValidationErrors = errors };
                return new BadRequestObjectResult(response);
            };

        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""

            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
                 {
                     new OpenApiSecurityScheme
                     {
                         Reference = new OpenApiReference
                         {
                             Type=ReferenceType.SecurityScheme,
                             Id="Bearer"
                         }
                     },
                     new List<string>()
                 }
             });
        });

        AddJwtAuthentication(services, configuration);
        return services;
    }

    private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                var jwt = configuration.GetSection("JWTOptions").Get<JWTOptions>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey))
                };
            });
    }
}
