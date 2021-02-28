﻿using Esquio.Abstractions;
using Esquio.AspNetCore.Diagnostics;
using Esquio.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.AspNetCore.Endpoints
{
    internal class EsquioClientEndpointMiddleware
    {
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        const string FEATURENAME_QUERY_PARAMETER_NAME = "featureName";
        const string DEFAULT_MIME_TYPE = MediaTypeNames.Application.Json;

        private readonly RequestDelegate _next;

        public EsquioClientEndpointMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }
        public async Task Invoke(HttpContext context, IFeatureService featureService, EsquioAspNetCoreDiagnostics diagnostics)
        {
            var evaluationsResponse = new List<EvaluationResponse>();

            var names = context.Request
                .Query[FEATURENAME_QUERY_PARAMETER_NAME];

            foreach (var featureName in names)
            {
                try
                {
                    diagnostics.EsquioClientMiddlewareEvaluatingFeature(featureName);

                    var isEnabled = await featureService
                        .IsEnabledAsync(featureName, context?.RequestAborted ?? CancellationToken.None);

                    evaluationsResponse.Add(new EvaluationResponse()
                    {
                        Name = featureName,
                        Enabled = isEnabled
                    });
                }
                catch (Exception exception)
                {
                    diagnostics.EsquioClientMiddlewareThrow(featureName, exception);

                    await WriteError(context, featureName);

                    return;
                }
            }

            diagnostics.EsquioClientMiddlewareSuccess();
            await WriteResponse(context, evaluationsResponse);
        }

        private async Task WriteResponse(HttpContext currentContext, IEnumerable<EvaluationResponse> response)
        {
            await WriteAsync(
                currentContext,
                JsonSerializer.Serialize(response, options: _serializerOptions),
                DEFAULT_MIME_TYPE,
                StatusCodes.Status200OK);
        }

        private async Task WriteError(HttpContext currentContext, string featureName)
        {
            await WriteAsync(
                currentContext,
                JsonSerializer.Serialize(EvaluationError.Default(featureName), options: _serializerOptions),
                DEFAULT_MIME_TYPE,
                StatusCodes.Status500InternalServerError);
        }

        private async Task WriteAsync(
           HttpContext context,
           string content,
           string contentType,
           int statusCode)
        {
            context.Response.Headers["Content-Type"] = new[] { contentType };
            context.Response.Headers["Cache-Control"] = new[] { "no-cache, no-store, must-revalidate" };
            context.Response.Headers["Pragma"] = new[] { "no-cache" };
            context.Response.Headers["Expires"] = new[] { "0" };
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(content);
        }

        private class EvaluationResponse
        {
            public bool Enabled { get; set; }

            public string Name { get; set; }
        }

        private class EvaluationError
        {
            public string Message { get; set; }

            private EvaluationError() { }

            public static EvaluationError Default(string featureName)
            {
                return new EvaluationError()
                {
                    Message = $"{nameof(OnErrorBehavior)} behavior for Esquio is configured to {nameof(OnErrorBehavior.Throw)} and middleware throw when check the state for {featureName}."
                    + $"You can modify this behavior using {nameof(EsquioOptions.ConfigureOnErrorBehavior)} method on AddEsquio options."
                };
            }
        }
    }
}
