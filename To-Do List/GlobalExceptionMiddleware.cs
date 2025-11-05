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
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                Success = false,
                ErrorMessage = "Внутренняя ошибка сервера"
            };

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

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

            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}