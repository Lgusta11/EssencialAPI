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
            // Verifica se o usuário possui o token e a função correta
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await RespondWithUnauthorizedAsync(context, "Token inválido");
                return;
            }

            // Verifica se o usuário tem a função necessária (exemplo: Admin)
            if (!context.User.IsInRole("Admin"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await RespondWithForbiddenAsync(context, "Acesso negado para o usuário atual");
                return;
            }

            // Continua com a execução da requisição se tudo estiver OK
            await _next(context);

            // Verifica se a resposta foi 401 e retorna a mensagem customizada
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                await RespondWithUnauthorizedAsync(context, "Token inválido");
            }
        }

        private async Task RespondWithUnauthorizedAsync(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                message,
                error = "Unauthorized",
                statusCode = 401
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }

        private async Task RespondWithForbiddenAsync(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                message,
                error = "Forbidden",
                statusCode = 403
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}