﻿using Esquio.CliTool.Internal;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;

namespace Esquio.CliTool.Command
{
    [Command(Constants.TogglesCommandName, Description = Constants.TogglesDescriptionCommandName),
        Subcommand(typeof(GetCommand)),
        Subcommand(typeof(ListCommand))]
    internal class TogglesCommand
    {
        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp(usePager: true);

            return 1;
        }

        private class ListCommand
        {
            [Option("--product-name <PRODUCT-NAME>", Description = "The product name.")]
            [Required]
            public string ProductName { get; set; }

            [Option("--feature-name <FEATURE-NAME>", Description = "The feature name to be rolled off.")]
            [Required]
            public string FeatureName { get; set; }

            [Option(Constants.UriParameter, Description = Constants.UriDescription)]
            public string Uri { get; set; } = Environment.GetEnvironmentVariable(Constants.UriEnvironmentVariable);

            [Option(Constants.ApiKeyParameter, Description = Constants.ApiKeyDescription)]
            public string ApiKey { get; set; } = Environment.GetEnvironmentVariable(Constants.ApiKeyEnvironmentVariable);

            private async Task<int> OnExecute(IConsole console)
            {
                var defaultForegroundColor = console.ForegroundColor;

                using (var client = EsquioClient.Create(
                    uri: Uri ?? Constants.UriDefaultValue,
                    apikey: ApiKey))
                {
                    var response = await client.ListTogglesAsync(ProductName,FeatureName);

                    if (response.IsSuccessStatusCode)
                    {
                        console.ForegroundColor = Constants.SuccessColor;
                        console.WriteLine(await response.GetContentDetailAsync());
                        console.ForegroundColor = defaultForegroundColor;

                        return 0;
                    }
                    else
                    {
                        console.ForegroundColor = Constants.ErrorColor;
                        console.WriteLine(await response.GetErrorDetailAsync());
                        console.ForegroundColor = defaultForegroundColor;

                        return 1;
                    }
                }
            }
        }

        private class GetCommand
        {
            [Option("--product-name <PRODUCT-NAME>", Description = "The product name.")]
            [Required]
            public string ProductName { get; set; }

            [Option("--feature-name <FEATURE-NAME>", Description = "The feature name to be rolled off.")]
            [Required]
            public string FeatureName { get; set; }

            [Option("--toggle-type <TOGGLE-TYPE>", Description = "The toggle identifier.")]
            [Required]
            public string ToggleType { get; set; }

            [Option(Constants.UriParameter, Description = Constants.UriDescription)]
            public string Uri { get; set; } = Environment.GetEnvironmentVariable(Constants.UriEnvironmentVariable);

            [Option(Constants.ApiKeyParameter, Description = Constants.ApiKeyDescription)]
            public string ApiKey { get; set; } = Environment.GetEnvironmentVariable(Constants.ApiKeyEnvironmentVariable);

            private async Task<int> OnExecute(IConsole console)
            {
                var defaultForegroundColor = console.ForegroundColor;

                using (var client = EsquioClient.Create(
                    uri: Uri ?? Constants.UriDefaultValue,
                    apikey: ApiKey))
                {
                    var response = await client.GetToggleAsync(ProductName, FeatureName, ToggleType);

                    if (response.IsSuccessStatusCode)
                    {
                        console.ForegroundColor = Constants.SuccessColor;
                        console.WriteLine(await response.GetContentDetailAsync());
                        console.ForegroundColor = defaultForegroundColor;

                        return 0;
                    }
                    else
                    {
                        console.ForegroundColor = Constants.ErrorColor;
                        console.WriteLine(await response.GetErrorDetailAsync());
                        console.ForegroundColor = defaultForegroundColor;

                        return 1;
                    }
                }
            }
        }
    }
}