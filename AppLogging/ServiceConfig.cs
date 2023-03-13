using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json.Serialization;

namespace Microsoft.AzurePercept.Agents.Telemetry
{
    public class ServiceConfig
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("namespace")]
        public string NameSpace { get; set; }

        public string GetSourceName()
        {
            return NameSpace + "." + Name;
        }
        public ActivitySource GetActivitySource()
        {
            return new ActivitySource(GetSourceName());
        }

        public Meter GetMeter()
        {
            return new Meter(NameSpace + '.' + Name);
        }

        public Dictionary<string, object> GetResourceAttributes()
        {
            return new Dictionary<string, object>
            {
                { "service.name", Name },
                { "service.namespace", NameSpace }
            };
        }
    }
}