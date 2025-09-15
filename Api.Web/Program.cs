using Api.Web.Middlewares;
using Persistence;
using Services;
using Swashbuckle.AspNetCore.SwaggerUI;
namespace Api.Web;
public class Program
{
    public static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
        });

        builder.Services.AddApplicationServices(builder.Configuration)
                        .AddWebApplicationServices(builder.Configuration)
                        .AddInfrastructureServices(builder.Configuration);
       
        var app = builder.Build();
        await app.InitializeDataBaseAsync();
        app.UseCustomExceptionHandler();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = "Shift";
                options.DocExpansion(DocExpansion.None);
                options.EnableFilter();
                options.DisplayRequestDuration();
            });
        }
        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }


}



