using Loujico.BL;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Loujico
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
     //   private readonly Ilog _log;

        public ErrorHandlingMiddleware(RequestDelegate next/*, Ilog log*/)
        {
            _next = next;
         //   _log = log;
        }
        private string GetClientIp(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            // optional: read X-Forwarded-For for full chain
            var xff = context.Request.Headers["X-Forwarded-For"].ToString();
            return $"{ip} (XFF: {xff})";
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var log = context.RequestServices.GetRequiredService<Ilog>();
            try
            {
                await _next(context);
                if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
                {
                    var info = new
                    {
                        TimeUtc = DateTime.UtcNow,
                        Ip = GetClientIp(context),
                        Path = context.Request.Path,
                        Method = context.Request.Method,

                    };
                    await log.Add("Error", $"unAuthorize : {@info} ", null);
                }
             
            }
            catch (Exception ex)
            {
                var info = new
                {
                    TimeUtc = DateTime.UtcNow,
                    Ip = GetClientIp(context),
                    Path = context.Request.Path,
                    Exception = ex.Message
                };
                await log.Add("Error",$"{@info}", null);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var log = context.RequestServices.GetRequiredService<Ilog>();
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case UnauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    message = "Access denied. You are not authorized.";
                    break;

                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            // تسجيل الخطأ
            await log.Add("Error", exception.Message, null);

            // تجهيز الرد
            var result = JsonSerializer.Serialize(new
            {
                error = message,
                status = (int)status
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            await context.Response.WriteAsync(result);
        }
    }
  
}
