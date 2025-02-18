namespace StationnementAPI.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _apiKey = configuration["ApiSettings:ApiKey"]; // Récupération de la clé d’API
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Clé API manquante.");
                return;
            }

            if (!_apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Clé API invalide.");
                return;
            }

            await _next(context);
        }
    }
}
