﻿using Esquio.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Seedwork;
using Xunit;

namespace UnitTests.Esquio.AspNetCore.Mvc
{
    public class FlagTagHelperShould
    {
        [Fact]
        public async Task clean_content_when_feature_is_not_active()
        {
            var featureService = new DelegatedFeatureService((_, __) => false);
            var logger = new LoggerFactory().CreateLogger<FlagTagHelper>();

            var tagHelper = new FlagTagHelper(featureService, logger)
            {
                FeatureName = "Feature-1"
            };

            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput(tag: "flag", innerContent: "<p>some content</p>");

            await tagHelper.ProcessAsync(context, output);

            output.Content.IsEmptyOrWhiteSpace
                .Should()
                .BeTrue();
        }
        [Fact]
        public async Task preserve_content_when_feature_is_active()
        {
            var featureService = new DelegatedFeatureService((_, __) => true);
            var logger = new LoggerFactory().CreateLogger<FlagTagHelper>();

            var tagHelper = new FlagTagHelper(featureService, logger)
            {
                FeatureName = "Feature-1"
            };

            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput(tag: "flag", innerContent: "<p>some content</p>");

            await tagHelper.ProcessAsync(context, output);

            output.Content.GetContent().Should().Contain("some content");
        }
        private TagHelperContext CreateTagHelperContext()
        {
            return new TagHelperContext(
                new TagHelperAttributeList(Enumerable.Empty<TagHelperAttribute>()),
                new Dictionary<object, object>(),
                "test");
        }
        private TagHelperOutput CreateTagHelperOutput(string tag, string innerContent)
        {
            var output = new TagHelperOutput(tag,
               new TagHelperAttributeList(),
               (useCachedResult_, encoder) =>
               {
                   var tagHelperContent = new DefaultTagHelperContent();

                   tagHelperContent.SetContent(innerContent);

                   return Task.FromResult<TagHelperContent>(tagHelperContent);
               });

            output.PreContent.SetContent("precontent");
            output.Content.SetContent(innerContent);
            output.PostContent.SetContent("postcontent");

            return output;
        }
    }
}
