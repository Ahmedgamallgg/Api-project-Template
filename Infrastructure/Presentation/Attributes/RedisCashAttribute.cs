using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Presentation.Attributes;
internal class RedisCashAttribute(int durationInSec = 90)
    : ActionFilterAttribute
{
    // 
    public override async Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var service = context.HttpContext.RequestServices.GetRequiredService<ICasheService>();
        string cashKey = CreateCashKey(context.HttpContext.Request);
        var cashValue = await service.GetAsync(cashKey);
        if (cashValue != null)
        {
            context.Result = new ContentResult
            {
                Content = cashValue,
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK
            };
            return;
        }
        var executedContext = await next.Invoke();
        if (executedContext.Result is OkObjectResult result)
            await service.SetAsync(cashKey,
                result.Value, TimeSpan.FromSeconds(durationInSec));
    }

    private static string CreateCashKey(HttpRequest request)
    {
        StringBuilder builder = new();

        builder.Append(request.Path + '?');
        foreach (var item in request.Query.OrderBy(q => q.Key))
            builder.Append($"{item.Key}={item.Value}&");
        return builder.ToString().Trim('&');
    }
}
