using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace StationnementAPI.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, string> _apiKeys; // Stocke les clés API

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _apiKeys = configuration.GetSection("ApiSettings:ApiKeys").Get<Dictionary<string, string>>(); // Récupère les clés API configurées.
        }

        public async Task Invoke(HttpContext context)
        {
            // 1️ Vérifier si l'en-tête "ApiKey" est présent
            if (!context.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Clé API manquante.");
                return;
            }

            // 2️ Vérifier si l'en-tête "X-Client-Type" (nom du programme) est présent
            if (!context.Request.Headers.TryGetValue("X-Client-Type", out var clientType))
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Identifiant du programme (X-Client-Type) manquant.");
                return;
            }

            // 3️ Vérifier si la clé API correspond bien au programme qui la réclame
            if (!_apiKeys.TryGetValue(clientType, out var expectedApiKey) || expectedApiKey != extractedApiKey)
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("⛔ Clé API invalide ou ne correspond pas au programme.");
                return;
            }

            Console.WriteLine($"Authentification réussie pour {clientType}");
            await _next(context);
        }
    }
}
