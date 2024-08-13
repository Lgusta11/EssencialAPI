using System.Text.Json;

namespace WebAPIUdemy.DTOs.AuthDTO
{
    public class UnauthorizedResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Token inválido",
                    error = "Unauthorized",
                    statusCode = 401
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
