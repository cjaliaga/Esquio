﻿using Esquio.AspNetCore.Endpoints;
using Esquio.AspNetCore.Endpoints.Metadata;
using Microsoft.AspNetCore.Routing;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Provides Esquio extensions methods for <see cref="IEndpointRouteBuilder"/>
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Specify a feature check to the endpoint(s).
        /// </summary>
        /// <typeparam name="TBuilder"><see cref="IEndpointConventionBuilder"/></typeparam>
        /// <param name="builder">The endopoint convention builder.</param>
        /// <param name="name">The feature name to be evaluated.</param>
        /// <returns>The original convention builder to be chained.</returns>
        public static TBuilder RequireFeature<TBuilder>(this TBuilder builder, string name) where TBuilder : IEndpointConventionBuilder
        {
            builder.Add(endpointbuilder =>
            {
                var metadata = new FeatureFilter(name);

                endpointbuilder.Metadata
                    .Add(metadata);
            });

            return builder;
        }

        /// <summary>
        /// Map a new endpoint that can  be used to get the activation state of any configured feature.
        /// </summary>
        /// <param name="endpoints"><see cref="IEndpointRouteBuilder"/></param>
        /// <param name="pattern">The uri pattern for this endpoint.</param>
        /// <returns>A <see cref="IEndpointRouteBuilder"/> to continue configuring this new endpoint.</returns>
        public static IEndpointConventionBuilder MapEsquio(this IEndpointRouteBuilder endpoints, string pattern = "esquio")
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            var pipeline = endpoints.CreateApplicationBuilder()
                .UseMiddleware<EsquioClientEndpointMiddleware>()
                .Build();

            return endpoints.MapGet(pattern, pipeline);
        }

        /// <summary>
        /// Map a new endpoint that can be used to get the information about the custom toggles used on this product.
        /// </summary>
        /// <param name="endpoints"><see cref="IEndpointRouteBuilder"/></param>
        /// <param name="pattern">The uri pattern for this endpoint.</param>
        /// <returns>A <see cref="IEndpointRouteBuilder"/> to continue configuring this new endpoint.</returns>
        public static IEndpointConventionBuilder MapEsquioDiscoverCustomTogglesDefinitions(this IEndpointRouteBuilder endpoints, string pattern = "esquio-toggles")
        {
            if (endpoints == null )
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            var pipeline = endpoints.CreateApplicationBuilder()
                .UseMiddleware<EsquioDiscoverCustomTogglesDefinitionMiddleware>()
                .Build();

            return endpoints.MapGet(pattern, pipeline);
        }
    }
}