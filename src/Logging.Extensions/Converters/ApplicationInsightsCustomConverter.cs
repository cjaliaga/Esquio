﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

namespace Logging.Extensions.Converters
{
    public class ApplicationInsightsCustomConverter : TraceTelemetryConverter
    {
        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            // first create a default TraceTelemetry using the sink's default logic
            // .. but without the log level, and (rendered) message (template) included in the Properties
            foreach (var telemetry in base.Convert(logEvent, formatProvider))
            {
                // then go ahead and post-process the telemetry's context to contain the user id as desired
                if (logEvent.Properties.ContainsKey(Constants.UserId))
                {
                    telemetry.Context.User.Id = logEvent.Properties[Constants.UserId].ToString().Trim('"');
                }
                // post-process the telemetry's context to contain the operation id
                if (logEvent.Properties.ContainsKey("operation_Id"))
                {
                    telemetry.Context.Operation.Id = logEvent.Properties["operation_Id"].ToString();
                }
                // post-process the telemetry's context to contain the operation parent id
                if (logEvent.Properties.ContainsKey("operation_parentId"))
                {
                    telemetry.Context.Operation.ParentId = logEvent.Properties["operation_parentId"].ToString();
                }
                // typecast to ISupportProperties so you can manipulate the properties as desired
                var propTelematry = telemetry as ISupportProperties;

                // find redundent properties
                var removeProps = new[] { "UserId", "operation_parentId", "operation_Id" };
                removeProps = removeProps.Where(prop => propTelematry.Properties.ContainsKey(prop)).ToArray();

                foreach (var prop in removeProps)
                {
                    // remove redundent properties
                    propTelematry.Properties.Remove(prop);
                }

                yield return telemetry;
            }
        }
    }
}
