using FluentValidation;
using System.Net;

namespace NasaSyncService.API.Middlewares
{
    public class ExceptionWrappingMiddleware(
        RequestDelegate requestDelegate,
        ILogger<ExceptionWrappingMiddleware> logger)
    {
        private readonly RequestDelegate _requestDelegate = requestDelegate;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _requestDelegate(httpContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Request processing error.");
                await HandleExeptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExeptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = (int)GetErrorCode(ex);
            var responseDate = GetResponseDate(ex);

            return httpContext.Response.WriteAsync(responseDate);
        }

        private static HttpStatusCode GetErrorCode(Exception ex)
        {
            return ex switch
            {
                ArgumentNullException or ValidationException => HttpStatusCode.BadRequest,
                NotImplementedException => HttpStatusCode.NotImplemented,
                _ => HttpStatusCode.InternalServerError,
            };
        }

        private static string GetResponseDate(Exception ex)
        {
            return ex switch
            {
                ValidationException => ex.Message,
                _ => "Something went wrong...",
            };
        }
    }

    public static class ExeptionWrappingMiddlewareExtention
    {
        public static IApplicationBuilder UseExeptionWrappingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionWrappingMiddleware>();
        }
    }
}
