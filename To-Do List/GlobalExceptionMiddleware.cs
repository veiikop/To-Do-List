using System.Net;
using System.Text.Json;

namespace To_Do_List.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Необработанное исключение");
                await HandleExceptionAsync(context, exception);
            }

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    title = "Forbidden",
                    status = 403,
                    detail = "Недостаточно прав для доступа к ресурсу"
                };
                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await context.Response.WriteAsync(json);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new { Success = false, ErrorMessage = "Внутренняя ошибка сервера" };
            var statusCode = HttpStatusCode.InternalServerError;

            if (exception is KeyNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
                response = new { Success = false, ErrorMessage = "Ресурс не найден" };
            }
            else if (exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
                response = new { Success = false, ErrorMessage = exception.Message };
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                response = new { Success = false, ErrorMessage = "Доступ запрещен" };
            }

            context.Response.StatusCode = (int)statusCode;
            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}